using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Stock;
using Pos.Domain.Entities;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class StockRepository : IStockRepository
{
    private readonly PosDbContext _dbContext;

    public StockRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ProductExistsAsync(Guid tenantId, Guid productId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Productos.AsNoTracking()
            .AnyAsync(p => p.TenantId == tenantId && p.Id == productId, cancellationToken);
    }

    public async Task<StockConfigDto?> GetStockConfigAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductoStockConfigs.AsNoTracking()
            .Where(c => c.TenantId == tenantId && c.SucursalId == sucursalId && c.ProductoId == productId)
            .Select(c => new StockConfigDto(c.ProductoId, c.SucursalId, c.StockMinimo, c.StockDeseado, c.ToleranciaPct))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<StockConfigDto> UpsertStockConfigAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid productId,
        decimal stockMinimo,
        decimal stockDeseado,
        decimal toleranciaPct,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.ProductoStockConfigs
            .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.SucursalId == sucursalId && c.ProductoId == productId, cancellationToken);

        if (existing is null)
        {
            existing = new ProductoStockConfig(
                Guid.NewGuid(),
                tenantId,
                productId,
                sucursalId,
                stockMinimo,
                stockDeseado,
                toleranciaPct,
                nowUtc);
            _dbContext.ProductoStockConfigs.Add(existing);
        }
        else
        {
            existing.Update(stockMinimo, stockDeseado, toleranciaPct, nowUtc);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new StockConfigDto(existing.ProductoId, existing.SucursalId, existing.StockMinimo, existing.StockDeseado, existing.ToleranciaPct);
    }

    public async Task<IReadOnlyList<StockSaldoDto>> GetSaldosAsync(
        Guid tenantId,
        Guid sucursalId,
        string? search,
        Guid? proveedorId,
        CancellationToken cancellationToken = default)
    {
        var productsQuery = _dbContext.Productos.AsNoTracking()
            .Where(p => p.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            productsQuery = productsQuery.Where(p =>
                EF.Functions.ILike(p.Name, $"%{term}%")
                || EF.Functions.ILike(p.Sku, $"%{term}%"));
        }

        var productsBase = await productsQuery
            .OrderBy(p => p.Name)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Sku,
                LegacyProveedorId = p.ProveedorId
            })
            .ToListAsync(cancellationToken);

        if (productsBase.Count == 0)
        {
            return Array.Empty<StockSaldoDto>();
        }

        var allProductIds = productsBase.Select(p => p.Id).ToList();

        var proveedorRelations = await _dbContext.ProductoProveedores.AsNoTracking()
            .Where(pp => pp.TenantId == tenantId && allProductIds.Contains(pp.ProductoId))
            .OrderByDescending(pp => pp.EsPrincipal)
            .Select(pp => new { pp.ProductoId, pp.ProveedorId, pp.EsPrincipal })
            .ToListAsync(cancellationToken);

        var proveedorByProduct = proveedorRelations
            .GroupBy(pp => pp.ProductoId)
            .ToDictionary(g => g.Key, g => (Guid?)g.First().ProveedorId);

        var resolvedProducts = productsBase
            .Select(p =>
            {
                proveedorByProduct.TryGetValue(p.Id, out var relProveedorId);
                var finalProveedorId = relProveedorId ?? p.LegacyProveedorId;
                return new
                {
                    p.Id,
                    p.Name,
                    p.Sku,
                    ProveedorId = finalProveedorId
                };
            })
            .Where(p => !proveedorId.HasValue || p.ProveedorId == proveedorId.Value)
            .ToList();

        if (resolvedProducts.Count == 0)
        {
            return Array.Empty<StockSaldoDto>();
        }

        var providerIds = resolvedProducts
            .Where(p => p.ProveedorId.HasValue)
            .Select(p => p.ProveedorId!.Value)
            .Distinct()
            .ToList();

        var providerNames = await _dbContext.Proveedores.AsNoTracking()
            .Where(pr => pr.TenantId == tenantId && providerIds.Contains(pr.Id))
            .Select(pr => new { pr.Id, pr.Name })
            .ToDictionaryAsync(pr => pr.Id, pr => pr.Name, cancellationToken);

        var productIds = resolvedProducts.Select(p => p.Id).ToList();

        var existingSaldos = await _dbContext.StockSaldos
            .Where(s => s.TenantId == tenantId && s.SucursalId == sucursalId && productIds.Contains(s.ProductoId))
            .ToListAsync(cancellationToken);

        var saldoByProduct = existingSaldos.ToDictionary(s => s.ProductoId, s => s);

        var now = DateTimeOffset.UtcNow;
        var added = false;

        foreach (var product in resolvedProducts)
        {
            if (saldoByProduct.ContainsKey(product.Id))
            {
                continue;
            }

            var saldo = new StockSaldo(Guid.NewGuid(), tenantId, product.Id, sucursalId, 0m, now);
            _dbContext.StockSaldos.Add(saldo);
            saldoByProduct[product.Id] = saldo;
            added = true;
        }

        if (added)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        var result = resolvedProducts.Select(p =>
        {
            var saldo = saldoByProduct[p.Id];
            var proveedorNombre = p.ProveedorId.HasValue && providerNames.TryGetValue(p.ProveedorId.Value, out var name)
                ? name
                : null;
            return new StockSaldoDto(
                p.Id,
                p.Name,
                p.Sku,
                p.Sku,
                saldo.CantidadActual,
                p.ProveedorId,
                proveedorNombre);
        }).ToList();

        return result;
    }

    public async Task<IReadOnlyList<StockAlertaDto>> GetAlertasAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid? proveedorId,
        CancellationToken cancellationToken = default)
    {
        var configs = await (from c in _dbContext.ProductoStockConfigs.AsNoTracking()
                join p in _dbContext.Productos.AsNoTracking().Where(p => p.TenantId == tenantId)
                    on c.ProductoId equals p.Id
                join pp in _dbContext.ProductoProveedores.AsNoTracking()
                    on new { p.Id, p.TenantId } equals new { Id = pp.ProductoId, pp.TenantId } into ppJoin
                from pp in ppJoin.Where(x => x.EsPrincipal).DefaultIfEmpty()
                join pr in _dbContext.Proveedores.AsNoTracking().Where(pr => pr.TenantId == tenantId)
                    on pp.ProveedorId equals pr.Id into prov
                from pr in prov.DefaultIfEmpty()
                join s in _dbContext.StockSaldos.AsNoTracking()
                    on new { c.ProductoId, c.TenantId, c.SucursalId }
                    equals new { s.ProductoId, s.TenantId, s.SucursalId } into saldoJoin
                from saldo in saldoJoin.DefaultIfEmpty()
                where c.TenantId == tenantId && c.SucursalId == sucursalId && p.IsActive
                select new
                {
                    p.Id,
                    p.Name,
                    p.Sku,
                    ProveedorId = pp != null ? (Guid?)pp.ProveedorId : null,
                    Proveedor = pr != null ? pr.Name : null,
                    ProveedorTelefono = pr != null ? pr.Telefono : null,
                    ProveedorCuit = pr != null ? pr.Cuit : null,
                    ProveedorDireccion = pr != null ? pr.Direccion : null,
                    StockActual = saldo != null ? saldo.CantidadActual : 0m,
                    c.StockMinimo,
                    c.StockDeseado,
                    c.ToleranciaPct
                })
            .ToListAsync(cancellationToken);

        if (proveedorId.HasValue)
        {
            configs = configs
                .Where(c => c.ProveedorId == proveedorId.Value)
                .ToList();
        }

        var alertas = new List<StockAlertaDto>();
        foreach (var item in configs)
        {
            var limite = item.StockMinimo * (1 + (item.ToleranciaPct / 100m));
            var nivel = item.StockActual <= item.StockMinimo
                ? "CRITICO"
                : item.StockActual <= limite
                    ? "BAJO"
                    : null;

            if (nivel is null)
            {
                continue;
            }

            var sugerido = Math.Max(item.StockDeseado - item.StockActual, 0m);
            alertas.Add(new StockAlertaDto(
                item.Id,
                item.Name,
                item.Sku,
                item.ProveedorId,
                item.Proveedor,
                item.StockActual,
                item.StockMinimo,
                item.StockDeseado,
                item.ToleranciaPct,
                sugerido,
                nivel));
        }

        return alertas.OrderBy(a => a.Nombre).ToList();
    }

    public async Task<StockSugeridoCompraDto> GetSugeridoCompraAsync(
        Guid tenantId,
        Guid sucursalId,
        CancellationToken cancellationToken = default)
    {
        var configs = await (from c in _dbContext.ProductoStockConfigs.AsNoTracking()
                join p in _dbContext.Productos.AsNoTracking().Where(p => p.TenantId == tenantId)
                    on c.ProductoId equals p.Id
                join pp in _dbContext.ProductoProveedores.AsNoTracking()
                    on new { p.Id, p.TenantId } equals new { Id = pp.ProductoId, pp.TenantId } into ppJoin
                from pp in ppJoin.Where(x => x.EsPrincipal).DefaultIfEmpty()
                join pr in _dbContext.Proveedores.AsNoTracking().Where(pr => pr.TenantId == tenantId)
                    on pp.ProveedorId equals pr.Id into prov
                from pr in prov.DefaultIfEmpty()
                join s in _dbContext.StockSaldos.AsNoTracking()
                    on new { c.ProductoId, c.TenantId, c.SucursalId }
                    equals new { s.ProductoId, s.TenantId, s.SucursalId } into saldoJoin
                from saldo in saldoJoin.DefaultIfEmpty()
                where c.TenantId == tenantId && c.SucursalId == sucursalId && p.IsActive
                select new
                {
                    p.Id,
                    p.Name,
                    p.Sku,
                    ProveedorId = pp != null ? (Guid?)pp.ProveedorId : null,
                    Proveedor = pr != null ? pr.Name : null,
                    StockActual = saldo != null ? saldo.CantidadActual : 0m,
                    c.StockMinimo,
                    c.ToleranciaPct
                })
            .ToListAsync(cancellationToken);

        var candidates = configs
            .Where(x => x.StockActual <= x.StockMinimo * (1 + (x.ToleranciaPct / 100m)))
            .ToList();

        if (!candidates.Any())
        {
            return new StockSugeridoCompraDto(0m, 0, Array.Empty<StockSugeridoProveedorDto>());
        }

        var items = candidates.Select(c =>
        {
            var sugerido = Math.Max((c.StockMinimo * (1 + (c.ToleranciaPct / 100m))) - c.StockActual, 0m);
            return new
            {
                c.ProveedorId,
                Proveedor = string.IsNullOrWhiteSpace(c.Proveedor) ? "SIN PROVEEDOR" : c.Proveedor!,
                Item = new StockSugeridoItemDto(
                    c.Id,
                    c.Name,
                    c.Sku,
                    c.Sku,
                    c.StockActual,
                    c.StockMinimo,
                    sugerido)
            };
        }).ToList();

        var grupos = items
            .GroupBy(i => new { i.ProveedorId, i.Proveedor })
            .Select(g =>
            {
                var list = g.Select(x => x.Item).OrderBy(x => x.Nombre).ToList();
                return new StockSugeridoProveedorDto(
                    g.Key.ProveedorId,
                    g.Key.Proveedor,
                    list.Sum(i => i.Sugerido),
                    list.Count(),
                    list);
            })
            .OrderBy(g => g.Proveedor)
            .ToList();

        return new StockSugeridoCompraDto(
            grupos.Sum(g => g.TotalSugerido),
            grupos.Sum(g => g.TotalItems),
            grupos);
    }

    public async Task<IReadOnlyList<StockRemitoProductoDto>> GetProductosRemitoAsync(
        Guid tenantId,
        IReadOnlyCollection<Guid> productoIds,
        Guid? proveedorId,
        CancellationToken cancellationToken = default)
    {
        if (productoIds.Count == 0)
        {
            return Array.Empty<StockRemitoProductoDto>();
        }

        var products = await (from p in _dbContext.Productos.AsNoTracking()
                join pp in _dbContext.ProductoProveedores.AsNoTracking()
                    on new { p.Id, p.TenantId } equals new { Id = pp.ProductoId, pp.TenantId } into ppJoin
                from pp in ppJoin.Where(x => x.EsPrincipal).DefaultIfEmpty()
                join pr in _dbContext.Proveedores.AsNoTracking().Where(pr => pr.TenantId == tenantId)
                    on pp.ProveedorId equals pr.Id into prov
                from pr in prov.DefaultIfEmpty()
                where p.TenantId == tenantId && productoIds.Contains(p.Id)
                select new
                {
                    p.Id,
                    p.Name,
                    p.Sku,
                    ProveedorId = pp != null ? (Guid?)pp.ProveedorId : null,
                    Proveedor = pr != null ? pr.Name : null,
                    ProveedorTelefono = pr != null ? pr.Telefono : null,
                    ProveedorCuit = pr != null ? pr.Cuit : null,
                    ProveedorDireccion = pr != null ? pr.Direccion : null
                })
            .ToListAsync(cancellationToken);

        if (proveedorId.HasValue)
        {
            products = products
                .Where(p => p.ProveedorId == proveedorId.Value)
                .ToList();
        }

        return products.Select(p =>
            new StockRemitoProductoDto(
                p.Id,
                p.Name,
                p.Sku,
                p.ProveedorId,
                p.Proveedor,
                p.ProveedorTelefono,
                p.ProveedorCuit,
                p.ProveedorDireccion))
            .ToList();
    }

    public async Task<StockRemitoHeaderDto> GetRemitoHeaderAsync(
        Guid tenantId,
        Guid sucursalId,
        CancellationToken cancellationToken = default)
    {
        var tenantName = await _dbContext.Tenants.AsNoTracking()
            .Where(t => t.Id == tenantId)
            .Select(t => t.Name)
            .FirstOrDefaultAsync(cancellationToken) ?? "Empresa";

        var sucursalName = await _dbContext.Sucursales.AsNoTracking()
            .Where(s => s.TenantId == tenantId && s.Id == sucursalId)
            .Select(s => s.Name)
            .FirstOrDefaultAsync(cancellationToken) ?? "Sucursal";

        return new StockRemitoHeaderDto(tenantName, sucursalName);
    }
}
