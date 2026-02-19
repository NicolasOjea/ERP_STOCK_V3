using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.ListasPrecio;
using Pos.Domain.Entities;
using Pos.Domain.Exceptions;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class ListaPrecioRepository : IListaPrecioRepository
{
    private readonly PosDbContext _dbContext;

    public ListaPrecioRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ListaPrecioDto>> GetListAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ListasPrecio.AsNoTracking()
            .Where(l => l.TenantId == tenantId)
            .OrderBy(l => l.Nombre)
            .Select(l => new ListaPrecioDto(l.Id, l.Nombre, l.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<ListaPrecioDto?> GetByIdAsync(
        Guid tenantId,
        Guid listaPrecioId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ListasPrecio.AsNoTracking()
            .Where(l => l.TenantId == tenantId && l.Id == listaPrecioId)
            .Select(l => new ListaPrecioDto(l.Id, l.Nombre, l.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateAsync(
        Guid tenantId,
        ListaPrecioCreateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var exists = await _dbContext.ListasPrecio.AsNoTracking()
            .AnyAsync(l => l.TenantId == tenantId && l.Nombre == request.Nombre, cancellationToken);

        if (exists)
        {
            throw new ConflictException("La lista de precio ya existe.");
        }

        var entity = new ListaPrecio(Guid.NewGuid(), tenantId, request.Nombre, nowUtc, request.IsActive ?? true);
        _dbContext.ListasPrecio.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(
        Guid tenantId,
        Guid listaPrecioId,
        ListaPrecioUpdateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ListasPrecio
            .FirstOrDefaultAsync(l => l.TenantId == tenantId && l.Id == listaPrecioId, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        var newNombre = request.Nombre ?? entity.Nombre;
        if (!string.Equals(newNombre, entity.Nombre, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _dbContext.ListasPrecio.AsNoTracking()
                .AnyAsync(l => l.TenantId == tenantId && l.Nombre == newNombre, cancellationToken);

            if (exists)
            {
                throw new ConflictException("La lista de precio ya existe.");
            }
        }

        var newIsActive = request.IsActive ?? entity.IsActive;
        entity.Update(newNombre, newIsActive, nowUtc);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task UpsertItemsAsync(
        Guid tenantId,
        Guid listaPrecioId,
        IReadOnlyList<ListaPrecioItemUpsertDto> items,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var listaExists = await _dbContext.ListasPrecio.AsNoTracking()
            .AnyAsync(l => l.TenantId == tenantId && l.Id == listaPrecioId, cancellationToken);

        if (!listaExists)
        {
            throw new NotFoundException("Lista de precio no encontrada.");
        }

        var productIds = items.Select(i => i.ProductoId).Distinct().ToList();
        var existingProducts = await _dbContext.Productos.AsNoTracking()
            .Where(p => p.TenantId == tenantId && productIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (existingProducts.Count != productIds.Count)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["productoId"] = new[] { "Producto no existe." }
                });
        }

        var existingItems = await _dbContext.ListaPrecioItems
            .Where(i => i.TenantId == tenantId && i.ListaPrecioId == listaPrecioId && productIds.Contains(i.ProductoId))
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            var existing = existingItems.FirstOrDefault(i => i.ProductoId == item.ProductoId);
            if (existing is null)
            {
                _dbContext.ListaPrecioItems.Add(new ListaPrecioItem(
                    Guid.NewGuid(),
                    tenantId,
                    listaPrecioId,
                    item.ProductoId,
                    item.Precio,
                    nowUtc));
            }
            else
            {
                existing.UpdatePrecio(item.Precio, nowUtc);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
