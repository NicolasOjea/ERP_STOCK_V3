using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Proveedores;
using Pos.Domain.Entities;
using Pos.Domain.Exceptions;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class ProveedorRepository : IProveedorRepository
{
    private readonly PosDbContext _dbContext;

    public ProveedorRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProveedorDto>> SearchAsync(
        Guid tenantId,
        string? search,
        bool? activo,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Proveedores.AsNoTracking()
            .Where(p => p.TenantId == tenantId);

        if (activo.HasValue)
        {
            query = query.Where(p => p.IsActive == activo.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(p =>
                EF.Functions.ILike(p.Name, $"%{term}%")
                || EF.Functions.ILike(p.Telefono, $"%{term}%")
                || (p.Cuit != null && EF.Functions.ILike(p.Cuit, $"%{term}%")));
        }

        return await query
            .OrderBy(p => p.Name)
            .Select(p => new ProveedorDto(p.Id, p.Name, p.Telefono, p.Cuit, p.Direccion, p.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(
        Guid tenantId,
        ProveedorCreateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var proveedor = new Proveedor(
            Guid.NewGuid(),
            tenantId,
            request.Name,
            request.Telefono,
            request.Cuit,
            request.Direccion,
            nowUtc,
            request.IsActive ?? true);
        _dbContext.Proveedores.Add(proveedor);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return proveedor.Id;
    }

    public async Task<bool> UpdateAsync(
        Guid tenantId,
        Guid proveedorId,
        ProveedorUpdateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var proveedor = await _dbContext.Proveedores
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == proveedorId, cancellationToken);

        if (proveedor is null)
        {
            return false;
        }

        var newName = request.Name is null ? proveedor.Name : request.Name;
        var newTelefono = request.Telefono is null ? proveedor.Telefono : request.Telefono;
        var newCuit = request.Cuit is null ? proveedor.Cuit : request.Cuit;
        var newDireccion = request.Direccion is null ? proveedor.Direccion : request.Direccion;
        var newIsActive = request.IsActive ?? proveedor.IsActive;

        proveedor.Update(newName, newTelefono, newCuit, newDireccion, newIsActive, nowUtc);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ProveedorDto?> GetByIdAsync(
        Guid tenantId,
        Guid proveedorId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Proveedores.AsNoTracking()
            .Where(p => p.TenantId == tenantId && p.Id == proveedorId)
            .Select(p => new ProveedorDto(p.Id, p.Name, p.Telefono, p.Cuit, p.Direccion, p.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProveedorDeleteProductOptionDto>> GetDeleteProductOptionsAsync(
        Guid tenantId,
        Guid proveedorId,
        CancellationToken cancellationToken = default)
    {
        var products = await _dbContext.Productos.AsNoTracking()
            .Where(p => p.TenantId == tenantId && p.ProveedorId == proveedorId)
            .OrderBy(p => p.Name)
            .Select(p => new { p.Id, p.Name, p.Sku })
            .ToListAsync(cancellationToken);

        if (products.Count == 0)
        {
            return Array.Empty<ProveedorDeleteProductOptionDto>();
        }

        var productIds = products.Select(p => p.Id).ToList();
        var usedProductIds = await GetUsedProductIdsAsync(tenantId, productIds, cancellationToken);

        return products
            .Select(p =>
            {
                var hasUsage = usedProductIds.Contains(p.Id);
                return new ProveedorDeleteProductOptionDto(
                    p.Id,
                    p.Name,
                    p.Sku,
                    !hasUsage,
                    hasUsage ? "Tiene ventas o movimientos asociados." : null);
            })
            .ToList();
    }

    public async Task<bool> DeleteAsync(
        Guid tenantId,
        Guid proveedorId,
        CancellationToken cancellationToken = default)
    {
        var proveedor = await _dbContext.Proveedores
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == proveedorId, cancellationToken);

        if (proveedor is null)
        {
            return false;
        }

        var hasUsage =
            await _dbContext.Productos.AsNoTracking().AnyAsync(p => p.TenantId == tenantId && p.ProveedorId == proveedorId, cancellationToken)
            || await _dbContext.ProductoProveedores.AsNoTracking().AnyAsync(r => r.TenantId == tenantId && r.ProveedorId == proveedorId, cancellationToken)
            || await _dbContext.OrdenesCompra.AsNoTracking().AnyAsync(o => o.TenantId == tenantId && o.ProveedorId == proveedorId, cancellationToken)
            || await _dbContext.DocumentosCompra.AsNoTracking().AnyAsync(d => d.TenantId == tenantId && d.ProveedorId == proveedorId, cancellationToken);

        if (hasUsage)
        {
            throw new ConflictException("No se puede eliminar el proveedor porque tiene productos o compras asociadas.");
        }

        _dbContext.Proveedores.Remove(proveedor);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteWithProductResolutionAsync(
        Guid tenantId,
        Guid proveedorId,
        IReadOnlyCollection<Guid> productIdsToDelete,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var proveedor = await _dbContext.Proveedores
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == proveedorId, cancellationToken);

        if (proveedor is null)
        {
            return false;
        }

        var hasCompras =
            await _dbContext.OrdenesCompra.AsNoTracking().AnyAsync(o => o.TenantId == tenantId && o.ProveedorId == proveedorId, cancellationToken)
            || await _dbContext.DocumentosCompra.AsNoTracking().AnyAsync(d => d.TenantId == tenantId && d.ProveedorId == proveedorId, cancellationToken);

        if (hasCompras)
        {
            throw new ConflictException("No se puede eliminar el proveedor porque tiene compras asociadas.");
        }

        var products = await _dbContext.Productos
            .Where(p => p.TenantId == tenantId && p.ProveedorId == proveedorId)
            .ToListAsync(cancellationToken);

        var productIds = products.Select(p => p.Id).ToList();
        var usedProductIds = await GetUsedProductIdsAsync(tenantId, productIds, cancellationToken);
        var selected = productIdsToDelete.Distinct().ToHashSet();

        var invalidSelected = selected.Where(id => !productIds.Contains(id)).ToList();
        if (invalidSelected.Count > 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["productIdsToDelete"] = new[] { "Hay productos seleccionados que no pertenecen al proveedor." }
                });
        }

        var blocked = selected.Where(usedProductIds.Contains).ToList();
        if (blocked.Count > 0)
        {
            throw new ConflictException("No se pueden borrar productos seleccionados porque tienen ventas o movimientos asociados.");
        }

        await using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var productsToDelete = products.Where(p => selected.Contains(p.Id)).ToList();
        var productsToKeep = products.Where(p => !selected.Contains(p.Id)).ToList();

        if (productsToDelete.Count > 0)
        {
            var deleteIds = productsToDelete.Select(p => p.Id).ToList();

            var codigos = await _dbContext.ProductoCodigos
                .Where(c => c.TenantId == tenantId && deleteIds.Contains(c.ProductoId))
                .ToListAsync(cancellationToken);
            var relaciones = await _dbContext.ProductoProveedores
                .Where(r => r.TenantId == tenantId && deleteIds.Contains(r.ProductoId))
                .ToListAsync(cancellationToken);
            var configs = await _dbContext.ProductoStockConfigs
                .Where(c => c.TenantId == tenantId && deleteIds.Contains(c.ProductoId))
                .ToListAsync(cancellationToken);
            var saldos = await _dbContext.StockSaldos
                .Where(s => s.TenantId == tenantId && deleteIds.Contains(s.ProductoId))
                .ToListAsync(cancellationToken);
            var listaPrecioItems = await _dbContext.ListaPrecioItems
                .Where(i => i.TenantId == tenantId && deleteIds.Contains(i.ProductoId))
                .ToListAsync(cancellationToken);

            _dbContext.ProductoCodigos.RemoveRange(codigos);
            _dbContext.ProductoProveedores.RemoveRange(relaciones);
            _dbContext.ProductoStockConfigs.RemoveRange(configs);
            _dbContext.StockSaldos.RemoveRange(saldos);
            _dbContext.ListaPrecioItems.RemoveRange(listaPrecioItems);
            _dbContext.Productos.RemoveRange(productsToDelete);
        }

        if (productsToKeep.Count > 0)
        {
            foreach (var product in productsToKeep)
            {
                product.SetProveedorPrincipal(null, nowUtc);
            }

            var keepIds = productsToKeep.Select(p => p.Id).ToList();
            var providerRelations = await _dbContext.ProductoProveedores
                .Where(r => r.TenantId == tenantId && r.ProveedorId == proveedorId && keepIds.Contains(r.ProductoId))
                .ToListAsync(cancellationToken);
            _dbContext.ProductoProveedores.RemoveRange(providerRelations);
        }

        _dbContext.Proveedores.Remove(proveedor);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return true;
    }

    private async Task<HashSet<Guid>> GetUsedProductIdsAsync(
        Guid tenantId,
        IReadOnlyCollection<Guid> productIds,
        CancellationToken cancellationToken)
    {
        if (productIds.Count == 0)
        {
            return new HashSet<Guid>();
        }
        var idsList = productIds.ToList();

        var used = new HashSet<Guid>();

        async Task AddFromAsync(IQueryable<Guid> query)
        {
            var ids = await query.ToListAsync(cancellationToken);
            foreach (var id in ids)
            {
                used.Add(id);
            }
        }

        await AddFromAsync(_dbContext.VentaItems.AsNoTracking()
            .Where(i => i.TenantId == tenantId && idsList.Contains(i.ProductoId))
            .Select(i => i.ProductoId));
        await AddFromAsync(_dbContext.StockMovimientoItems.AsNoTracking()
            .Where(i => i.TenantId == tenantId && idsList.Contains(i.ProductoId))
            .Select(i => i.ProductoId));
        await AddFromAsync(_dbContext.RecepcionItems.AsNoTracking()
            .Where(i => i.TenantId == tenantId && idsList.Contains(i.ProductoId))
            .Select(i => i.ProductoId));
        await AddFromAsync(_dbContext.DevolucionItems.AsNoTracking()
            .Where(i => i.TenantId == tenantId && idsList.Contains(i.ProductoId))
            .Select(i => i.ProductoId));
        await AddFromAsync(_dbContext.OrdenCompraItems.AsNoTracking()
            .Where(i => i.TenantId == tenantId && idsList.Contains(i.ProductoId))
            .Select(i => i.ProductoId));
        await AddFromAsync(_dbContext.PreRecepcionItems.AsNoTracking()
            .Where(i => i.TenantId == tenantId && i.ProductoId.HasValue && idsList.Contains(i.ProductoId.Value))
            .Select(i => i.ProductoId!.Value));

        return used;
    }
}
