using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Reportes;
using Pos.Domain.Enums;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class ReportesRepository : IReportesRepository
{
    private readonly PosDbContext _dbContext;

    public ReportesRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ReportResumenVentasDto> GetResumenVentasAsync(
        Guid tenantId,
        Guid sucursalId,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken = default)
    {
        var ventasQuery = _dbContext.Ventas.AsNoTracking()
            .Where(v => v.TenantId == tenantId && v.SucursalId == sucursalId && v.Estado == VentaEstado.Confirmada);

        if (desde.HasValue)
        {
            ventasQuery = ventasQuery.Where(v => v.UpdatedAt >= desde.Value);
        }

        if (hasta.HasValue)
        {
            ventasQuery = ventasQuery.Where(v => v.UpdatedAt <= hasta.Value);
        }

        var ventas = await ventasQuery
            .Select(v => new { v.Id, v.TotalNeto, v.Facturada })
            .ToListAsync(cancellationToken);

        var totalIngresos = ventas.Sum(v => v.TotalNeto);
        var totalFacturado = ventas.Where(v => v.Facturada).Sum(v => v.TotalNeto);
        var totalNoFacturado = ventas.Where(v => !v.Facturada).Sum(v => v.TotalNeto);

        var costoProductosIngresadosQuery =
            from item in _dbContext.StockMovimientoItems.AsNoTracking()
            join mov in _dbContext.StockMovimientos.AsNoTracking() on item.MovimientoId equals mov.Id
            join producto in _dbContext.Productos.AsNoTracking() on item.ProductoId equals producto.Id
            where item.TenantId == tenantId
                  && mov.TenantId == tenantId
                  && mov.SucursalId == sucursalId
                  && producto.TenantId == tenantId
                  && item.EsIngreso
            select new
            {
                Monto = item.Cantidad * producto.PrecioBase,
                mov.Fecha
            };

        if (desde.HasValue)
        {
            costoProductosIngresadosQuery = costoProductosIngresadosQuery.Where(x => x.Fecha >= desde.Value);
        }

        if (hasta.HasValue)
        {
            costoProductosIngresadosQuery = costoProductosIngresadosQuery.Where(x => x.Fecha <= hasta.Value);
        }

        var costoProductosIngresados = await costoProductosIngresadosQuery.SumAsync(x => (decimal?)x.Monto, cancellationToken) ?? 0m;

        var egresosCajaQuery = _dbContext.CajaMovimientos.AsNoTracking()
            .Where(m => m.TenantId == tenantId
                        && (m.Tipo == CajaMovimientoTipo.Retiro
                            || m.Tipo == CajaMovimientoTipo.Gasto
                            || m.Tipo == CajaMovimientoTipo.Egreso));

        if (desde.HasValue)
        {
            egresosCajaQuery = egresosCajaQuery.Where(x => x.Fecha >= desde.Value);
        }

        if (hasta.HasValue)
        {
            egresosCajaQuery = egresosCajaQuery.Where(x => x.Fecha <= hasta.Value);
        }

        var egresosCaja = await egresosCajaQuery.SumAsync(x => (decimal?)x.Monto, cancellationToken) ?? 0m;

        var diferenciasNegativasQuery = _dbContext.CajaSesiones.AsNoTracking()
            .Where(s => s.TenantId == tenantId
                        && s.SucursalId == sucursalId
                        && s.CierreAt != null
                        && s.DiferenciaTotal < 0);

        if (desde.HasValue)
        {
            diferenciasNegativasQuery = diferenciasNegativasQuery.Where(x => x.CierreAt >= desde.Value);
        }

        if (hasta.HasValue)
        {
            diferenciasNegativasQuery = diferenciasNegativasQuery.Where(x => x.CierreAt <= hasta.Value);
        }

        var diferenciasNegativas = await diferenciasNegativasQuery.SumAsync(x => (decimal?)(-x.DiferenciaTotal), cancellationToken) ?? 0m;

        var totalEgresos = costoProductosIngresados + egresosCaja + diferenciasNegativas;

        return new ReportResumenVentasDto(totalIngresos, totalEgresos, totalFacturado, totalNoFacturado);
    }

    public async Task<IReadOnlyList<VentaPorDiaItemDto>> GetVentasPorDiaAsync(
        Guid tenantId,
        Guid sucursalId,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Ventas.AsNoTracking()
            .Where(v => v.TenantId == tenantId && v.SucursalId == sucursalId && v.Estado == VentaEstado.Confirmada);

        if (desde.HasValue)
        {
            query = query.Where(v => v.UpdatedAt >= desde.Value);
        }

        if (hasta.HasValue)
        {
            query = query.Where(v => v.UpdatedAt <= hasta.Value);
        }

        // Npgsql/EF can fail translating DateTimeOffset.Date in GroupBy.
        // Materialize filtered rows and group by UTC day in memory.
        var raw = await query
            .Select(v => new { v.UpdatedAt, v.TotalNeto })
            .ToListAsync(cancellationToken);

        var result = raw
            .GroupBy(v => v.UpdatedAt.UtcDateTime.Date)
            .Select(g => new VentaPorDiaItemDto(g.Key, g.Sum(x => x.TotalNeto)))
            .OrderBy(x => x.Fecha)
            .ToList();

        return result;
    }

    public async Task<IReadOnlyList<MedioPagoItemDto>> GetMediosPagoAsync(
        Guid tenantId,
        Guid sucursalId,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken = default)
    {
        var query = from pago in _dbContext.VentaPagos.AsNoTracking()
            join venta in _dbContext.Ventas.AsNoTracking() on pago.VentaId equals venta.Id
            where pago.TenantId == tenantId
                  && venta.TenantId == tenantId
                  && venta.SucursalId == sucursalId
                  && venta.Estado == VentaEstado.Confirmada
            select new { pago.MedioPago, pago.Monto, venta.UpdatedAt };

        if (desde.HasValue)
        {
            query = query.Where(x => x.UpdatedAt >= desde.Value);
        }

        if (hasta.HasValue)
        {
            query = query.Where(x => x.UpdatedAt <= hasta.Value);
        }

        var raw = await query.ToListAsync(cancellationToken);

        var result = raw
            .GroupBy(x => x.MedioPago)
            .Select(g => new MedioPagoItemDto(g.Key, g.Sum(x => x.Monto)))
            .OrderByDescending(x => x.Total)
            .ToList();

        return result;
    }

    public async Task<IReadOnlyList<TopProductoItemDto>> GetTopProductosAsync(
        Guid tenantId,
        Guid sucursalId,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        int top,
        CancellationToken cancellationToken = default)
    {
        var query = from item in _dbContext.VentaItems.AsNoTracking()
            join venta in _dbContext.Ventas.AsNoTracking() on item.VentaId equals venta.Id
            join producto in _dbContext.Productos.AsNoTracking() on item.ProductoId equals producto.Id
            where item.TenantId == tenantId
                  && venta.TenantId == tenantId
                  && venta.SucursalId == sucursalId
                  && venta.Estado == VentaEstado.Confirmada
                  && producto.TenantId == tenantId
            select new
            {
                item.ProductoId,
                producto.Name,
                producto.Sku,
                item.Cantidad,
                item.PrecioUnitario,
                venta.UpdatedAt
            };

        if (desde.HasValue)
        {
            query = query.Where(x => x.UpdatedAt >= desde.Value);
        }

        if (hasta.HasValue)
        {
            query = query.Where(x => x.UpdatedAt <= hasta.Value);
        }

        var raw = await query.ToListAsync(cancellationToken);

        var result = raw
            .GroupBy(x => new { x.ProductoId, x.Name, x.Sku })
            .Select(g => new TopProductoItemDto(
                g.Key.ProductoId,
                g.Key.Name,
                g.Key.Sku,
                g.Sum(x => x.Cantidad),
                g.Sum(x => x.Cantidad * x.PrecioUnitario)))
            .OrderByDescending(x => x.Total)
            .Take(top)
            .ToList();

        return result;
    }

    public async Task<IReadOnlyList<RotacionStockItemDto>> GetRotacionStockAsync(
        Guid tenantId,
        Guid sucursalId,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken = default)
    {
        var query = from item in _dbContext.StockMovimientoItems.AsNoTracking()
            join mov in _dbContext.StockMovimientos.AsNoTracking() on item.MovimientoId equals mov.Id
            join producto in _dbContext.Productos.AsNoTracking() on item.ProductoId equals producto.Id
            where item.TenantId == tenantId
                  && mov.TenantId == tenantId
                  && mov.SucursalId == sucursalId
                  && producto.TenantId == tenantId
            select new
            {
                item.ProductoId,
                producto.Name,
                producto.Sku,
                item.Cantidad,
                item.EsIngreso,
                mov.Fecha
            };

        if (desde.HasValue)
        {
            query = query.Where(x => x.Fecha >= desde.Value);
        }

        if (hasta.HasValue)
        {
            query = query.Where(x => x.Fecha <= hasta.Value);
        }

        var raw = await query.ToListAsync(cancellationToken);

        var result = raw
            .GroupBy(x => new { x.ProductoId, x.Name, x.Sku })
            .Select(g => new RotacionStockItemDto(
                g.Key.ProductoId,
                g.Key.Name,
                g.Key.Sku,
                g.Where(x => x.EsIngreso).Sum(x => x.Cantidad),
                g.Where(x => !x.EsIngreso).Sum(x => x.Cantidad),
                g.Where(x => x.EsIngreso).Sum(x => x.Cantidad) - g.Where(x => !x.EsIngreso).Sum(x => x.Cantidad)))
            .OrderByDescending(x => x.Salidas)
            .ToList();

        return result;
    }

    public async Task<IReadOnlyList<StockInmovilizadoItemDto>> GetStockInmovilizadoAsync(
        Guid tenantId,
        Guid sucursalId,
        DateTimeOffset? desde,
        DateTimeOffset? hasta,
        CancellationToken cancellationToken = default)
    {
        var ventasItemsQuery =
            from item in _dbContext.VentaItems.AsNoTracking()
            join venta in _dbContext.Ventas.AsNoTracking() on item.VentaId equals venta.Id
            where item.TenantId == tenantId
                  && venta.TenantId == tenantId
                  && venta.SucursalId == sucursalId
                  && venta.Estado == VentaEstado.Confirmada
            select new
            {
                item.ProductoId,
                item.Cantidad,
                venta.UpdatedAt
            };

        if (desde.HasValue)
        {
            ventasItemsQuery = ventasItemsQuery.Where(x => x.UpdatedAt >= desde.Value);
        }

        if (hasta.HasValue)
        {
            ventasItemsQuery = ventasItemsQuery.Where(x => x.UpdatedAt <= hasta.Value);
        }

        var vendidosByProducto = await ventasItemsQuery
            .GroupBy(x => x.ProductoId)
            .Select(g => new
            {
                ProductoId = g.Key,
                CantidadVendida = g.Sum(x => x.Cantidad)
            })
            .ToDictionaryAsync(x => x.ProductoId, x => x.CantidadVendida, cancellationToken);

        var saldosQuery = _dbContext.StockSaldos.AsNoTracking()
            .Where(s => s.TenantId == tenantId && s.SucursalId == sucursalId);

        var saldosByProducto = await saldosQuery
            .GroupBy(s => s.ProductoId)
            .Select(g => new
            {
                ProductoId = g.Key,
                Stock = g.Sum(x => x.CantidadActual)
            })
            .ToDictionaryAsync(x => x.ProductoId, x => x.Stock, cancellationToken);

        var lastMovimientos = await (
            from item in _dbContext.StockMovimientoItems.AsNoTracking()
            join mov in _dbContext.StockMovimientos.AsNoTracking() on item.MovimientoId equals mov.Id
            where item.TenantId == tenantId
                  && mov.TenantId == tenantId
                  && mov.SucursalId == sucursalId
            group mov by item.ProductoId
            into g
            select new
            {
                ProductoId = g.Key,
                UltimoMovimiento = g.Max(x => x.Fecha)
            })
            .ToDictionaryAsync(x => x.ProductoId, x => (DateTimeOffset?)x.UltimoMovimiento, cancellationToken);

        var productos = await _dbContext.Productos.AsNoTracking()
            .Where(p => p.TenantId == tenantId && p.IsActive)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Sku,
                p.CreatedAt
            })
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        var nowUtc = DateTimeOffset.UtcNow;
        var result = productos
            .Where(p => !vendidosByProducto.TryGetValue(p.Id, out var qty) || qty <= 0)
            .Select(p =>
            {
                var ultimo = lastMovimientos.TryGetValue(p.Id, out var mov) ? mov : null;
                var stock = saldosByProducto.TryGetValue(p.Id, out var st) ? st : 0m;
                var baseDate = ultimo ?? p.CreatedAt;
                var diasSinMovimiento = Math.Max(0, (int)Math.Floor((nowUtc - baseDate).TotalDays));
                return new StockInmovilizadoItemDto(
                    p.Id,
                    p.Name,
                    p.Sku,
                    stock,
                    ultimo,
                    diasSinMovimiento);
            })
            .ToList();

        return result;
    }
}
