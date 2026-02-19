using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Caja;
using Pos.Application.DTOs.Devoluciones;
using Pos.Application.DTOs.Stock;
using Pos.Domain.Entities;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class DevolucionRepository : IDevolucionRepository
{
    private const string MedioPagoDefault = "EFECTIVO";

    private readonly PosDbContext _dbContext;

    public DevolucionRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DevolucionResultDto> CreateAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid userId,
        DevolucionCreateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var venta = await _dbContext.Ventas
            .FirstOrDefaultAsync(v => v.TenantId == tenantId && v.SucursalId == sucursalId && v.Id == request.VentaId, cancellationToken);

        if (venta is null)
        {
            throw new NotFoundException("Venta no encontrada.");
        }

        if (venta.Estado != VentaEstado.Confirmada)
        {
            throw new ConflictException("La venta no esta confirmada.");
        }

        var cajaSesion = await _dbContext.CajaSesiones
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.SucursalId == sucursalId && s.Estado == CajaSesionEstado.Abierta, cancellationToken);

        if (cajaSesion is null)
        {
            throw new ConflictException("No hay caja abierta.");
        }

        var ventaItems = await _dbContext.VentaItems.AsNoTracking()
            .Where(i => i.TenantId == tenantId && i.VentaId == venta.Id)
            .ToListAsync(cancellationToken);

        var ventaItemsByProduct = ventaItems.ToDictionary(i => i.ProductoId, i => i);

        var itemsAgrupados = request.Items
            .GroupBy(i => i.ProductoId)
            .Select(g => new { ProductoId = g.Key, Cantidad = g.Sum(x => x.Cantidad) })
            .ToList();

        foreach (var item in itemsAgrupados)
        {
            if (!ventaItemsByProduct.TryGetValue(item.ProductoId, out var ventaItem))
            {
                throw new NotFoundException("Producto no encontrado en la venta.");
            }

            if (item.Cantidad > ventaItem.Cantidad)
            {
                throw new ConflictException("Cantidad de devolucion supera lo vendido.");
            }
        }

        var productoIds = itemsAgrupados.Select(i => i.ProductoId).Distinct().ToList();
        var productos = await _dbContext.Productos.AsNoTracking()
            .Where(p => p.TenantId == tenantId && productoIds.Contains(p.Id))
            .Select(p => new { p.Id, p.Name, p.Sku })
            .ToListAsync(cancellationToken);

        if (productos.Count != productoIds.Count)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        var devolucionId = Guid.NewGuid();
        var total = itemsAgrupados.Sum(i => i.Cantidad * ventaItemsByProduct[i.ProductoId].PrecioUnitario);

        var devolucion = new Devolucion(
            devolucionId,
            tenantId,
            sucursalId,
            venta.Id,
            userId,
            request.Motivo,
            total,
            nowUtc);

        _dbContext.Devoluciones.Add(devolucion);

        var devolucionItems = new List<DevolucionItem>();
        foreach (var item in itemsAgrupados)
        {
            var ventaItem = ventaItemsByProduct[item.ProductoId];
            devolucionItems.Add(new DevolucionItem(
                Guid.NewGuid(),
                tenantId,
                devolucionId,
                item.ProductoId,
                item.Cantidad,
                ventaItem.PrecioUnitario,
                nowUtc));
        }

        _dbContext.DevolucionItems.AddRange(devolucionItems);

        var notaCredito = new NotaCreditoInterna(
            Guid.NewGuid(),
            tenantId,
            sucursalId,
            devolucionId,
            total,
            request.Motivo,
            nowUtc);

        _dbContext.NotasCreditoInterna.Add(notaCredito);

        var saldos = await _dbContext.StockSaldos
            .Where(s => s.TenantId == tenantId && s.SucursalId == sucursalId && productoIds.Contains(s.ProductoId))
            .ToListAsync(cancellationToken);

        var saldoByProduct = saldos.ToDictionary(s => s.ProductoId, s => s);
        var addedSaldo = false;
        foreach (var productId in productoIds)
        {
            if (saldoByProduct.ContainsKey(productId))
            {
                continue;
            }

            var saldo = new StockSaldo(Guid.NewGuid(), tenantId, productId, sucursalId, 0m, nowUtc);
            _dbContext.StockSaldos.Add(saldo);
            saldoByProduct[productId] = saldo;
            addedSaldo = true;
        }

        if (addedSaldo)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        var movimientoId = Guid.NewGuid();
        var movimiento = new StockMovimiento(
            movimientoId,
            tenantId,
            sucursalId,
            StockMovimientoTipo.EntradaDevolucion,
            $"Devolucion venta {venta.Id} - {request.Motivo}",
            nowUtc,
            nowUtc);

        _dbContext.StockMovimientos.Add(movimiento);

        var movimientoItems = new List<StockMovimientoItem>();
        var stockCambios = new List<StockSaldoChangeDto>();
        foreach (var item in itemsAgrupados)
        {
            var saldo = saldoByProduct[item.ProductoId];
            var before = saldo.CantidadActual;
            var after = before + item.Cantidad;
            saldo.SetCantidad(after, nowUtc);

            var movimientoItemId = Guid.NewGuid();
            movimientoItems.Add(new StockMovimientoItem(
                movimientoItemId,
                tenantId,
                movimientoId,
                item.ProductoId,
                item.Cantidad,
                true,
                nowUtc));

            stockCambios.Add(new StockSaldoChangeDto(
                movimientoId,
                movimientoItemId,
                item.ProductoId,
                before,
                after));
        }

        _dbContext.StockMovimientoItems.AddRange(movimientoItems);

        var movimientosSum = await _dbContext.CajaMovimientos
            .Where(m => m.TenantId == tenantId && m.CajaSesionId == cajaSesion.Id)
            .SumAsync(m => (decimal?)m.Monto, cancellationToken) ?? 0m;

        var saldoAntes = cajaSesion.MontoInicial + movimientosSum;
        var monto = -total;
        var saldoDespues = saldoAntes + monto;

        var cajaMovimiento = new CajaMovimiento(
            Guid.NewGuid(),
            tenantId,
            cajaSesion.Id,
            CajaMovimientoTipo.Egreso,
            MedioPagoDefault,
            monto,
            $"Devolucion venta {venta.Id} - {request.Motivo}",
            nowUtc,
            nowUtc);

        _dbContext.CajaMovimientos.Add(cajaMovimiento);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var itemsDto = devolucionItems
            .Select(item =>
            {
                var product = productos.Single(p => p.Id == item.ProductoId);
                var subtotal = item.Cantidad * item.PrecioUnitario;
                return new DevolucionItemDto(
                    item.Id,
                    item.ProductoId,
                    product.Name,
                    product.Sku,
                    item.Cantidad,
                    item.PrecioUnitario,
                    subtotal);
            })
            .ToList();

        var devolucionDto = new DevolucionDto(
            devolucion.Id,
            devolucion.VentaId,
            devolucion.Motivo,
            devolucion.Total,
            devolucion.CreatedAt,
            itemsDto);

        var notaDto = new NotaCreditoInternaDto(
            notaCredito.Id,
            notaCredito.DevolucionId,
            notaCredito.Total,
            notaCredito.Motivo,
            notaCredito.CreatedAt);

        var cajaMovimientoDto = new CajaMovimientoDto(
            cajaMovimiento.Id,
            cajaMovimiento.CajaSesionId,
            cajaMovimiento.Tipo.ToString().ToUpperInvariant(),
            cajaMovimiento.MedioPago,
            cajaMovimiento.Monto,
            cajaMovimiento.Motivo,
            cajaMovimiento.Fecha);

        var cajaResult = new CajaMovimientoResultDto(cajaMovimientoDto, saldoAntes, saldoDespues);

        return new DevolucionResultDto(
            devolucionDto,
            notaDto,
            stockCambios,
            new[] { cajaResult });
    }
}
