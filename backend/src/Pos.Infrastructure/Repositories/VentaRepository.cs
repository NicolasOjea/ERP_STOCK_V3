using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Ventas;
using Pos.Application.DTOs.Pricing;
using Pos.Application.DTOs.Stock;
using Pos.Application.DTOs.Caja;
using Pos.Domain.Entities;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class VentaRepository : IVentaRepository
{
    private readonly PosDbContext _dbContext;

    public VentaRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> CreateAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid userId,
        string listaPrecio,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var numero = await GetNextVentaNumeroAsync(cancellationToken);
        var venta = new Venta(Guid.NewGuid(), tenantId, sucursalId, userId, listaPrecio, nowUtc, numero);
        _dbContext.Ventas.Add(venta);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return venta.Id;
    }

    private async Task<long> GetNextVentaNumeroAsync(CancellationToken cancellationToken)
    {
        var next = await _dbContext.Database
            .SqlQuery<long>($"SELECT nextval('venta_numero_seq') AS \"Value\"")
            .SingleAsync(cancellationToken);
        return next;
    }

    public async Task<VentaDto?> GetByIdAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        CancellationToken cancellationToken = default)
    {
        var venta = await _dbContext.Ventas.AsNoTracking()
            .FirstOrDefaultAsync(v => v.TenantId == tenantId && v.SucursalId == sucursalId && v.Id == ventaId, cancellationToken);

        if (venta is null)
        {
            return null;
        }

        var items = await (from i in _dbContext.VentaItems.AsNoTracking()
                join p in _dbContext.Productos.AsNoTracking() on i.ProductoId equals p.Id
                where i.TenantId == tenantId && i.VentaId == ventaId && p.TenantId == tenantId
                orderby p.Name
                select new VentaItemDto(
                    i.Id,
                    i.ProductoId,
                    p.Name,
                    p.Sku,
                    i.Codigo,
                    i.Cantidad,
                    i.PrecioUnitario,
                    i.Cantidad * i.PrecioUnitario))
            .ToListAsync(cancellationToken);

        return new VentaDto(
            venta.Id,
            venta.Numero,
            venta.SucursalId,
            venta.UserId,
            venta.Estado.ToString().ToUpperInvariant(),
            venta.ListaPrecio,
            venta.TotalNeto,
            venta.TotalPagos,
            venta.CreatedAt,
            items,
            venta.Facturada);
    }

    public async Task<VentaTicketDto?> GetTicketByNumeroAsync(
        Guid tenantId,
        Guid sucursalId,
        long numeroVenta,
        CancellationToken cancellationToken = default)
    {
        var venta = await _dbContext.Ventas.AsNoTracking()
            .FirstOrDefaultAsync(
                v => v.TenantId == tenantId && v.SucursalId == sucursalId && v.Numero == numeroVenta,
                cancellationToken);

        if (venta is null)
        {
            return null;
        }

        var items = await (from i in _dbContext.VentaItems.AsNoTracking()
                join p in _dbContext.Productos.AsNoTracking() on i.ProductoId equals p.Id
                where i.TenantId == tenantId && i.VentaId == venta.Id && p.TenantId == tenantId
                orderby p.Name
                select new VentaItemDto(
                    i.Id,
                    i.ProductoId,
                    p.Name,
                    p.Sku,
                    i.Codigo,
                    i.Cantidad,
                    i.PrecioUnitario,
                    i.Cantidad * i.PrecioUnitario))
            .ToListAsync(cancellationToken);

        var pagos = await _dbContext.VentaPagos.AsNoTracking()
            .Where(vp => vp.TenantId == tenantId && vp.VentaId == venta.Id)
            .OrderBy(vp => vp.CreatedAt)
            .Select(vp => new VentaPagoDto(vp.Id, vp.MedioPago, vp.Monto))
            .ToListAsync(cancellationToken);

        var ventaDto = new VentaDto(
            venta.Id,
            venta.Numero,
            venta.SucursalId,
            venta.UserId,
            venta.Estado.ToString().ToUpperInvariant(),
            venta.ListaPrecio,
            venta.TotalNeto,
            venta.TotalPagos,
            venta.CreatedAt,
            items,
            venta.Facturada);

        return new VentaTicketDto(ventaDto, pagos);
    }

    public async Task<VentaItemChangeDto> AddItemByCodeAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        string code,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var venta = await _dbContext.Ventas
            .FirstOrDefaultAsync(v => v.TenantId == tenantId && v.SucursalId == sucursalId && v.Id == ventaId, cancellationToken);

        if (venta is null)
        {
            throw new NotFoundException("Venta no encontrada.");
        }

        if (venta.Estado != VentaEstado.Borrador)
        {
            throw new ConflictException("La venta no esta en borrador.");
        }

        var cajaAbierta = await _dbContext.CajaSesiones.AsNoTracking()
            .AnyAsync(s => s.TenantId == tenantId && s.SucursalId == sucursalId && s.Estado == CajaSesionEstado.Abierta, cancellationToken);
        if (!cajaAbierta)
        {
            throw new ConflictException("No hay caja abierta.");
        }

        var product = await _dbContext.Productos.AsNoTracking()
            .Where(p => p.TenantId == tenantId && p.Sku == code && p.IsActive)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Sku,
                p.PrecioBase,
                p.PrecioVenta,
                Codigo = p.Sku
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            throw new NotFoundException($"Producto no encontrado para SKU {code}");
        }

        var item = await _dbContext.VentaItems
            .FirstOrDefaultAsync(i => i.TenantId == tenantId && i.VentaId == ventaId && i.ProductoId == product.Id, cancellationToken);

        var creado = false;
        var cantidadAntes = 0m;
        var precioUnitario = 0m;
        if (item is null)
        {
            precioUnitario = await GetPrecioUnitarioAsync(
                tenantId,
                venta.ListaPrecio,
                product.Id,
                product.PrecioVenta,
                product.PrecioBase,
                cancellationToken);
            item = new VentaItem(Guid.NewGuid(), tenantId, ventaId, product.Id, product.Codigo, 1m, precioUnitario, nowUtc);
            _dbContext.VentaItems.Add(item);
            creado = true;
        }
        else
        {
            cantidadAntes = item.Cantidad;
            item.Incrementar(1m, nowUtc);
            precioUnitario = item.PrecioUnitario;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var dto = new VentaItemDto(
            item.Id,
            product.Id,
            product.Name,
            product.Sku,
            item.Codigo,
            item.Cantidad,
            precioUnitario,
            item.Cantidad * precioUnitario);

        return new VentaItemChangeDto(dto, cantidadAntes, item.Cantidad, creado);
    }

    public async Task<VentaItemChangeDto> AddItemByProductAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        Guid productId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var venta = await _dbContext.Ventas
            .FirstOrDefaultAsync(v => v.TenantId == tenantId && v.SucursalId == sucursalId && v.Id == ventaId, cancellationToken);

        if (venta is null)
        {
            throw new NotFoundException("Venta no encontrada.");
        }

        if (venta.Estado != VentaEstado.Borrador)
        {
            throw new ConflictException("La venta no esta en borrador.");
        }

        var cajaAbierta = await _dbContext.CajaSesiones.AsNoTracking()
            .AnyAsync(s => s.TenantId == tenantId && s.SucursalId == sucursalId && s.Estado == CajaSesionEstado.Abierta, cancellationToken);
        if (!cajaAbierta)
        {
            throw new ConflictException("No hay caja abierta.");
        }

        var product = await _dbContext.Productos.AsNoTracking()
            .Where(p => p.TenantId == tenantId && p.Id == productId && p.IsActive)
            .Select(p => new { p.Id, p.Name, p.Sku, p.PrecioBase, p.PrecioVenta })
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            throw new ConflictException("Producto inactivo o no encontrado.");
        }

        var codigoFinal = product.Sku;

        var item = await _dbContext.VentaItems
            .FirstOrDefaultAsync(i => i.TenantId == tenantId && i.VentaId == ventaId && i.ProductoId == product.Id, cancellationToken);

        var creado = false;
        var cantidadAntes = 0m;
        var precioUnitario = 0m;
        if (item is null)
        {
            precioUnitario = await GetPrecioUnitarioAsync(
                tenantId,
                venta.ListaPrecio,
                product.Id,
                product.PrecioVenta,
                product.PrecioBase,
                cancellationToken);
            item = new VentaItem(Guid.NewGuid(), tenantId, ventaId, product.Id, codigoFinal, 1m, precioUnitario, nowUtc);
            _dbContext.VentaItems.Add(item);
            creado = true;
        }
        else
        {
            cantidadAntes = item.Cantidad;
            item.Incrementar(1m, nowUtc);
            precioUnitario = item.PrecioUnitario;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var dto = new VentaItemDto(
            item.Id,
            product.Id,
            product.Name,
            product.Sku,
            item.Codigo,
            item.Cantidad,
            precioUnitario,
            item.Cantidad * precioUnitario);

        return new VentaItemChangeDto(dto, cantidadAntes, item.Cantidad, creado);
    }

    public async Task<VentaItemDto> RemoveItemAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        Guid itemId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var venta = await _dbContext.Ventas
            .FirstOrDefaultAsync(v => v.TenantId == tenantId && v.SucursalId == sucursalId && v.Id == ventaId, cancellationToken);

        if (venta is null)
        {
            throw new NotFoundException("Venta no encontrada.");
        }

        if (venta.Estado != VentaEstado.Borrador)
        {
            throw new ConflictException("La venta no esta en borrador.");
        }

        var item = await (from i in _dbContext.VentaItems
                join p in _dbContext.Productos.AsNoTracking() on i.ProductoId equals p.Id
                where i.TenantId == tenantId && i.VentaId == ventaId && i.Id == itemId && p.TenantId == tenantId
                select new { Item = i, p.Name, p.Sku })
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            throw new NotFoundException("Item no encontrado.");
        }

        _dbContext.VentaItems.Remove(item.Item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new VentaItemDto(
            item.Item.Id,
            item.Item.ProductoId,
            item.Name,
            item.Sku,
            item.Item.Codigo,
            item.Item.Cantidad,
            item.Item.PrecioUnitario,
            item.Item.Cantidad * item.Item.PrecioUnitario);
    }

    public async Task<VentaItemChangeDto> UpdateItemCantidadAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        Guid itemId,
        decimal cantidad,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var venta = await _dbContext.Ventas
            .FirstOrDefaultAsync(v => v.TenantId == tenantId && v.SucursalId == sucursalId && v.Id == ventaId, cancellationToken);

        if (venta is null)
        {
            throw new NotFoundException("Venta no encontrada.");
        }

        if (venta.Estado != VentaEstado.Borrador)
        {
            throw new ConflictException("La venta no esta en borrador.");
        }

        var item = await _dbContext.VentaItems
            .FirstOrDefaultAsync(i => i.TenantId == tenantId && i.VentaId == ventaId && i.Id == itemId, cancellationToken);

        if (item is null)
        {
            throw new NotFoundException("Item no encontrado.");
        }

        var cantidadAntes = item.Cantidad;
        item.ActualizarCantidad(cantidad, nowUtc);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var product = await _dbContext.Productos.AsNoTracking()
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == item.ProductoId, cancellationToken);

        var nombre = product?.Name ?? string.Empty;
        var sku = product?.Sku ?? string.Empty;

        var dto = new VentaItemDto(
            item.Id,
            item.ProductoId,
            nombre,
            sku,
            item.Codigo,
            item.Cantidad,
            item.PrecioUnitario,
            item.Cantidad * item.PrecioUnitario);
        return new VentaItemChangeDto(dto, cantidadAntes, item.Cantidad, false);
    }

    public async Task<VentaConfirmResultDto> ConfirmAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        VentaConfirmRequestDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var venta = await _dbContext.Ventas
            .FirstOrDefaultAsync(v => v.TenantId == tenantId && v.SucursalId == sucursalId && v.Id == ventaId, cancellationToken);

        if (venta is null)
        {
            throw new NotFoundException("Venta no encontrada.");
        }

        if (venta.Estado != VentaEstado.Borrador)
        {
            throw new ConflictException("La venta no esta en borrador.");
        }

        CajaSesion? cajaSesion;
        if (request.CajaSesionId.HasValue)
        {
            var cajaSesionId = request.CajaSesionId.Value;
            cajaSesion = await _dbContext.CajaSesiones
                .FirstOrDefaultAsync(
                    s => s.TenantId == tenantId
                         && s.SucursalId == sucursalId
                         && s.Id == cajaSesionId
                         && s.Estado == CajaSesionEstado.Abierta,
                    cancellationToken);

            if (cajaSesion is null)
            {
                throw new ConflictException("La caja indicada no esta abierta.");
            }
        }
        else
        {
            cajaSesion = await _dbContext.CajaSesiones
                .Where(s => s.TenantId == tenantId && s.SucursalId == sucursalId && s.Estado == CajaSesionEstado.Abierta)
                .OrderByDescending(s => s.AperturaAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (cajaSesion is null)
            {
                throw new ConflictException("No hay caja abierta.");
            }
        }

        var items = await _dbContext.VentaItems
            .Where(i => i.TenantId == tenantId && i.VentaId == ventaId)
            .ToListAsync(cancellationToken);

        var totalNeto = items.Sum(i => i.Cantidad * i.PrecioUnitario);

        var totalPagos = request.Pagos?.Sum(p => p.Monto) ?? 0m;
        if (totalNeto != totalPagos)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["pagos"] = new[] { "La suma de pagos debe ser igual al total." }
                });
        }

        if (totalNeto > 0 && (request.Pagos is null || request.Pagos.Count == 0))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["pagos"] = new[] { "Debe especificar pagos." }
                });
        }

        var productIds = items.Select(i => i.ProductoId).Distinct().ToList();
        var saldos = await _dbContext.StockSaldos
            .Where(s => s.TenantId == tenantId && s.SucursalId == sucursalId && productIds.Contains(s.ProductoId))
            .ToListAsync(cancellationToken);

        var saldoByProduct = saldos.ToDictionary(s => s.ProductoId, s => s);
        var stockCambios = new List<StockSaldoChangeDto>();

        foreach (var item in items)
        {
            if (!saldoByProduct.TryGetValue(item.ProductoId, out var saldo))
            {
                throw new ConflictException("Stock insuficiente.");
            }

            var before = saldo.CantidadActual;
            var after = before - item.Cantidad;
            if (after < 0)
            {
                throw new ConflictException("Stock insuficiente.");
            }

            saldo.SetCantidad(after, nowUtc);
        }

        var movimientoId = Guid.NewGuid();
        var movimiento = new StockMovimiento(
            movimientoId,
            tenantId,
            sucursalId,
            StockMovimientoTipo.SalidaVenta,
            $"Venta {venta.Numero}",
            nowUtc,
            nowUtc,
            venta.Numero);

        _dbContext.StockMovimientos.Add(movimiento);

        var movimientoItems = new List<StockMovimientoItem>();
        foreach (var item in items)
        {
            var saldo = saldoByProduct[item.ProductoId];
            var before = saldo.CantidadActual + item.Cantidad;
            var after = saldo.CantidadActual;

            var itemId = Guid.NewGuid();
            movimientoItems.Add(new StockMovimientoItem(
                itemId,
                tenantId,
                movimientoId,
                item.ProductoId,
                item.Cantidad,
                false,
                nowUtc));

            stockCambios.Add(new StockSaldoChangeDto(
                movimientoId,
                itemId,
                item.ProductoId,
                before,
                after));
        }

        _dbContext.StockMovimientoItems.AddRange(movimientoItems);

        venta.Confirmar(totalNeto, totalPagos, request.Facturada!.Value, nowUtc);

        var pagos = new List<VentaPago>();
        var pagosDto = new List<VentaPagoDto>();
        foreach (var pago in request.Pagos ?? Array.Empty<VentaPagoRequestDto>())
        {
            var pagoEntity = new VentaPago(
                Guid.NewGuid(),
                tenantId,
                ventaId,
                pago.MedioPago,
                pago.Monto,
                nowUtc);
            pagos.Add(pagoEntity);
            pagosDto.Add(new VentaPagoDto(pagoEntity.Id, pagoEntity.MedioPago, pagoEntity.Monto));
        }

        _dbContext.VentaPagos.AddRange(pagos);

        var cajaMovimientos = new List<CajaMovimientoResultDto>();

        var movimientosSum = await _dbContext.CajaMovimientos
            .Where(m => m.TenantId == tenantId && m.CajaSesionId == cajaSesion.Id)
            .SumAsync(m => (decimal?)m.Monto, cancellationToken) ?? 0m;

        var saldoActual = cajaSesion.MontoInicial + movimientosSum;

        foreach (var pago in request.Pagos ?? Array.Empty<VentaPagoRequestDto>())
        {
            var saldoAntes = saldoActual;
            saldoActual += pago.Monto;

            var movimientoCaja = new CajaMovimiento(
                Guid.NewGuid(),
                tenantId,
                cajaSesion.Id,
                CajaMovimientoTipo.Ingreso,
                pago.MedioPago,
                pago.Monto,
                $"Venta {ventaId}",
                nowUtc,
                nowUtc);

            _dbContext.CajaMovimientos.Add(movimientoCaja);

            var dto = new CajaMovimientoDto(
                movimientoCaja.Id,
                movimientoCaja.CajaSesionId,
                movimientoCaja.Tipo.ToString().ToUpperInvariant(),
                movimientoCaja.MedioPago,
                movimientoCaja.Monto,
                movimientoCaja.Motivo,
                movimientoCaja.Fecha);

            cajaMovimientos.Add(new CajaMovimientoResultDto(dto, saldoAntes, saldoActual));
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var ventaDto = await GetByIdAsync(tenantId, sucursalId, ventaId, cancellationToken)
            ?? throw new NotFoundException("Venta no encontrada.");

        return new VentaConfirmResultDto(
            ventaDto,
            pagosDto,
            stockCambios,
            cajaMovimientos);
    }

    public async Task<VentaAnularResultDto> AnularAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        VentaAnularRequestDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var venta = await _dbContext.Ventas
            .FirstOrDefaultAsync(v => v.TenantId == tenantId && v.SucursalId == sucursalId && v.Id == ventaId, cancellationToken);

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

        var items = await _dbContext.VentaItems
            .Where(i => i.TenantId == tenantId && i.VentaId == ventaId)
            .ToListAsync(cancellationToken);

        var pagos = await _dbContext.VentaPagos.AsNoTracking()
            .Where(p => p.TenantId == tenantId && p.VentaId == ventaId)
            .ToListAsync(cancellationToken);

        var productIds = items.Select(i => i.ProductoId).Distinct().ToList();
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
            StockMovimientoTipo.EntradaAnulacion,
            $"Anulacion venta {venta.Numero} - {request.Motivo}",
            nowUtc,
            nowUtc,
            venta.Numero);

        _dbContext.StockMovimientos.Add(movimiento);

        var movimientoItems = new List<StockMovimientoItem>();
        var stockCambios = new List<StockSaldoChangeDto>();
        foreach (var item in items)
        {
            var saldo = saldoByProduct[item.ProductoId];
            var before = saldo.CantidadActual;
            var after = before + item.Cantidad;
            saldo.SetCantidad(after, nowUtc);

            var itemId = Guid.NewGuid();
            movimientoItems.Add(new StockMovimientoItem(
                itemId,
                tenantId,
                movimientoId,
                item.ProductoId,
                item.Cantidad,
                true,
                nowUtc));

            stockCambios.Add(new StockSaldoChangeDto(
                movimientoId,
                itemId,
                item.ProductoId,
                before,
                after));
        }

        _dbContext.StockMovimientoItems.AddRange(movimientoItems);

        var movimientosSum = await _dbContext.CajaMovimientos
            .Where(m => m.TenantId == tenantId && m.CajaSesionId == cajaSesion.Id)
            .SumAsync(m => (decimal?)m.Monto, cancellationToken) ?? 0m;

        var saldoActual = cajaSesion.MontoInicial + movimientosSum;
        var cajaMovimientos = new List<CajaMovimientoResultDto>();

        foreach (var pago in pagos)
        {
            var saldoAntes = saldoActual;
            var monto = -pago.Monto;
            saldoActual += monto;

            var movimientoCaja = new CajaMovimiento(
                Guid.NewGuid(),
                tenantId,
                cajaSesion.Id,
                CajaMovimientoTipo.Egreso,
                pago.MedioPago,
                monto,
                $"Anulacion venta {ventaId} - {request.Motivo}",
                nowUtc,
                nowUtc);

            _dbContext.CajaMovimientos.Add(movimientoCaja);

            var dto = new CajaMovimientoDto(
                movimientoCaja.Id,
                movimientoCaja.CajaSesionId,
                movimientoCaja.Tipo.ToString().ToUpperInvariant(),
                movimientoCaja.MedioPago,
                movimientoCaja.Monto,
                movimientoCaja.Motivo,
                movimientoCaja.Fecha);

            cajaMovimientos.Add(new CajaMovimientoResultDto(dto, saldoAntes, saldoActual));
        }

        venta.CambiarEstado(VentaEstado.Anulada, nowUtc);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var ventaDto = await GetByIdAsync(tenantId, sucursalId, ventaId, cancellationToken)
            ?? throw new NotFoundException("Venta no encontrada.");

        return new VentaAnularResultDto(
            ventaDto,
            stockCambios,
            cajaMovimientos);
    }

    public async Task<VentaPricingSnapshotDto?> GetPricingSnapshotAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ventaId,
        CancellationToken cancellationToken = default)
    {
        var venta = await _dbContext.Ventas.AsNoTracking()
            .FirstOrDefaultAsync(v => v.TenantId == tenantId && v.SucursalId == sucursalId && v.Id == ventaId, cancellationToken);

        if (venta is null)
        {
            return null;
        }

        var items = await (from i in _dbContext.VentaItems.AsNoTracking()
                join p in _dbContext.Productos.AsNoTracking() on i.ProductoId equals p.Id
                where i.TenantId == tenantId && i.VentaId == ventaId && p.TenantId == tenantId
                orderby p.Name
                select new VentaPricingItemDto(
                    i.Id,
                    i.ProductoId,
                    p.CategoriaId,
                    p.Name,
                    p.Sku,
                    i.Cantidad,
                    i.PrecioUnitario))
            .ToListAsync(cancellationToken);

        return new VentaPricingSnapshotDto(
            venta.Id,
            venta.ListaPrecio,
            items);
    }

    private async Task<decimal> GetPrecioUnitarioAsync(
        Guid tenantId,
        string listaPrecio,
        Guid productoId,
        decimal precioVenta,
        decimal precioBase,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(listaPrecio))
        {
            var lista = await _dbContext.ListasPrecio.AsNoTracking()
                .FirstOrDefaultAsync(l => l.TenantId == tenantId && l.Nombre == listaPrecio, cancellationToken);

            if (lista is not null)
            {
                var item = await _dbContext.ListaPrecioItems.AsNoTracking()
                    .Where(i => i.TenantId == tenantId && i.ListaPrecioId == lista.Id && i.ProductoId == productoId)
                    .Select(i => new { i.Precio })
                    .FirstOrDefaultAsync(cancellationToken);

                if (item is not null)
                {
                    return item.Precio;
                }
            }
        }

        return precioVenta > 0 ? precioVenta : precioBase;
    }
}
