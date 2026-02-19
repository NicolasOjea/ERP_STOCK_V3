using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Stock;
using Pos.Domain.Entities;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class StockMovementRepository : IStockMovementRepository
{
    private readonly PosDbContext _dbContext;

    public StockMovementRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StockMovimientoRegisterResult> RegisterAsync(
        Guid tenantId,
        Guid sucursalId,
        StockMovimientoTipo tipo,
        string motivo,
        IReadOnlyCollection<StockMovimientoItemCreateDto> items,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var productIds = items.Select(i => i.ProductoId).Distinct().ToList();
        var products = await _dbContext.Productos.AsNoTracking()
            .Where(p => p.TenantId == tenantId && productIds.Contains(p.Id))
            .Select(p => new { p.Id, p.Name, p.Sku })
            .ToListAsync(cancellationToken);

        if (products.Count != productIds.Count)
        {
            throw new NotFoundException("Producto no encontrado.");
        }

        var saldos = await _dbContext.StockSaldos
            .Where(s => s.TenantId == tenantId && s.SucursalId == sucursalId && productIds.Contains(s.ProductoId))
            .ToListAsync(cancellationToken);

        var saldoByProduct = saldos.ToDictionary(s => s.ProductoId, s => s);
        var addedSaldo = false;
        foreach (var productId in productIds)
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
            tipo,
            motivo,
            nowUtc,
            nowUtc);

        _dbContext.StockMovimientos.Add(movimiento);

        var itemEntities = new List<StockMovimientoItem>();
        var itemDtos = new List<StockMovimientoItemDto>();
        var cambios = new List<StockSaldoChangeDto>();
        foreach (var item in items)
        {
            var saldo = saldoByProduct[item.ProductoId];
            var before = saldo.CantidadActual;
            var delta = item.EsIngreso ? item.Cantidad : -item.Cantidad;
            var after = before + delta;

            if (after < 0)
            {
                throw new ConflictException("Stock insuficiente.");
            }

            saldo.SetCantidad(after, nowUtc);

            var itemId = Guid.NewGuid();
            var entity = new StockMovimientoItem(
                itemId,
                tenantId,
                movimientoId,
                item.ProductoId,
                item.Cantidad,
                item.EsIngreso,
                nowUtc);
            itemEntities.Add(entity);

            var product = products.Single(p => p.Id == item.ProductoId);
            itemDtos.Add(new StockMovimientoItemDto(
                itemId,
                item.ProductoId,
                product.Name,
                product.Sku,
                item.Cantidad,
                item.EsIngreso,
                before,
                after));

            cambios.Add(new StockSaldoChangeDto(
                movimientoId,
                itemId,
                item.ProductoId,
                before,
                after));
        }

        _dbContext.StockMovimientoItems.AddRange(itemEntities);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var dto = new StockMovimientoDto(
            movimientoId,
            tipo.ToString().ToUpperInvariant(),
            motivo,
            nowUtc,
            null,
            null,
            itemDtos);

        return new StockMovimientoRegisterResult(dto, cambios);
    }

    public async Task<IReadOnlyList<StockMovimientoDto>> SearchAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid? productoId,
        long? ventaNumero,
        bool? facturada,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken = default)
    {
        var movimientosQuery = _dbContext.StockMovimientos.AsNoTracking()
            .Where(m => m.TenantId == tenantId && m.SucursalId == sucursalId);

        if (desde.HasValue)
        {
            movimientosQuery = movimientosQuery.Where(m => m.Fecha >= desde.Value);
        }

        if (hasta.HasValue)
        {
            movimientosQuery = movimientosQuery.Where(m => m.Fecha <= hasta.Value);
        }

        if (productoId.HasValue)
        {
            var pid = productoId.Value;
            movimientosQuery = movimientosQuery.Where(m =>
                _dbContext.StockMovimientoItems.AsNoTracking()
                    .Any(i => i.TenantId == tenantId && i.MovimientoId == m.Id && i.ProductoId == pid));
        }

        if (ventaNumero.HasValue)
        {
            var numero = ventaNumero.Value;
            movimientosQuery = movimientosQuery.Where(m => m.VentaNumero == numero);
        }

        if (facturada.HasValue)
        {
            var facturadaValue = facturada.Value;
            movimientosQuery = movimientosQuery.Where(m =>
                m.VentaNumero.HasValue
                && _dbContext.Ventas.AsNoTracking().Any(v =>
                    v.TenantId == tenantId
                    && v.SucursalId == sucursalId
                    && v.Numero == m.VentaNumero.Value
                    && v.Facturada == facturadaValue));
        }

        var movimientos = await movimientosQuery
            .OrderByDescending(m => m.Fecha)
            .ToListAsync(cancellationToken);

        if (movimientos.Count == 0)
        {
            return Array.Empty<StockMovimientoDto>();
        }

        var movimientoIds = movimientos.Select(m => m.Id).ToList();
        var numerosVenta = movimientos
            .Where(m => m.VentaNumero.HasValue)
            .Select(m => m.VentaNumero!.Value)
            .Distinct()
            .ToList();

        var facturadaByNumero = numerosVenta.Count == 0
            ? new Dictionary<long, bool>()
            : await _dbContext.Ventas.AsNoTracking()
                .Where(v => v.TenantId == tenantId && v.SucursalId == sucursalId && numerosVenta.Contains(v.Numero))
                .Select(v => new { v.Numero, v.Facturada })
                .ToDictionaryAsync(x => x.Numero, x => x.Facturada, cancellationToken);

        var items = await (from i in _dbContext.StockMovimientoItems.AsNoTracking()
                join p in _dbContext.Productos.AsNoTracking().Where(p => p.TenantId == tenantId)
                    on i.ProductoId equals p.Id
                where i.TenantId == tenantId && movimientoIds.Contains(i.MovimientoId)
                select new
                {
                    i,
                    p.Name,
                    p.Sku
                })
            .ToListAsync(cancellationToken);

        var itemsByMovimiento = items.GroupBy(x => x.i.MovimientoId)
            .ToDictionary(g => g.Key, g => g.Select(x => new StockMovimientoItemDto(
                x.i.Id,
                x.i.ProductoId,
                x.Name,
                x.Sku,
                x.i.Cantidad,
                x.i.EsIngreso,
                null,
                null)).ToList());

        var result = movimientos.Select(m =>
        {
            itemsByMovimiento.TryGetValue(m.Id, out var movimientoItems);
            var list = movimientoItems ?? new List<StockMovimientoItemDto>();
            return new StockMovimientoDto(
                m.Id,
                m.Tipo.ToString().ToUpperInvariant(),
                m.Motivo,
                m.Fecha,
                m.VentaNumero,
                m.VentaNumero.HasValue && facturadaByNumero.TryGetValue(m.VentaNumero.Value, out var ventaFacturada)
                    ? ventaFacturada
                    : null,
                list);
        }).ToList();

        return result;
    }
}
