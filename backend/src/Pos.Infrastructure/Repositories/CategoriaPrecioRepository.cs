using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Categorias;
using Pos.Domain.Entities;
using Pos.Domain.Enums;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class CategoriaPrecioRepository : ICategoriaPrecioRepository
{
    private readonly PosDbContext _dbContext;

    public CategoriaPrecioRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CategoriaPrecioDto>> SearchAsync(
        Guid tenantId,
        string? search,
        bool? activo,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Categorias.AsNoTracking()
            .Where(c => c.TenantId == tenantId);

        if (activo.HasValue)
        {
            query = query.Where(c => c.IsActive == activo.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c => EF.Functions.ILike(c.Name, $"%{term}%"));
        }

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new CategoriaPrecioDto(c.Id, c.Name, c.MargenGananciaPct, c.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoriaPrecioDto?> GetByIdAsync(
        Guid tenantId,
        Guid categoriaId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categorias.AsNoTracking()
            .Where(c => c.TenantId == tenantId && c.Id == categoriaId)
            .Select(c => new CategoriaPrecioDto(c.Id, c.Name, c.MargenGananciaPct, c.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(
        Guid tenantId,
        CategoriaPrecioCreateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var categoria = new Categoria(
            Guid.NewGuid(),
            tenantId,
            request.Name,
            nowUtc,
            request.MargenGananciaPct ?? 30m,
            request.IsActive ?? true);

        _dbContext.Categorias.Add(categoria);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return categoria.Id;
    }

    public async Task<bool> UpdateAsync(
        Guid tenantId,
        Guid categoriaId,
        CategoriaPrecioUpdateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var categoria = await _dbContext.Categorias
            .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Id == categoriaId, cancellationToken);
        if (categoria is null)
        {
            return false;
        }

        var newName = request.Name ?? categoria.Name;
        var newMargin = request.MargenGananciaPct ?? categoria.MargenGananciaPct;
        var newActive = request.IsActive ?? categoria.IsActive;
        var applyToProducts = request.AplicarAProductos ?? true;

        categoria.Update(newName, newMargin, newActive, nowUtc);

        if (applyToProducts && request.MargenGananciaPct.HasValue)
        {
            var products = await _dbContext.Productos
                .Where(p => p.TenantId == tenantId && p.CategoriaId == categoriaId && p.PricingMode == ProductPricingMode.Categoria)
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                var salePrice = CalculateSalePrice(product.PrecioBase, newMargin);
                product.Update(
                    product.Name,
                    product.Sku,
                    product.CategoriaId,
                    product.MarcaId,
                    product.ProveedorId,
                    product.PrecioBase,
                    salePrice,
                    ProductPricingMode.Categoria,
                    newMargin,
                    product.IsActive,
                    nowUtc);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return true;
    }

    private static decimal CalculateSalePrice(decimal precioBase, decimal margenPct)
    {
        if (precioBase < 0)
        {
            return 0;
        }

        var factor = 1m + (margenPct / 100m);
        var value = precioBase * factor;
        return decimal.Round(value, 4, MidpointRounding.AwayFromZero);
    }
}
