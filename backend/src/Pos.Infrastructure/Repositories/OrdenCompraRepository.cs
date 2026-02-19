using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Compras;
using Pos.Domain.Entities;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class OrdenCompraRepository : IOrdenCompraRepository
{
    private readonly PosDbContext _dbContext;

    public OrdenCompraRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> CreateAsync(
        Guid tenantId,
        Guid sucursalId,
        OrdenCompraCreateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        if (request.ProveedorId.HasValue)
        {
            var proveedorExists = await _dbContext.Proveedores.AsNoTracking()
                .AnyAsync(p => p.TenantId == tenantId && p.Id == request.ProveedorId.Value, cancellationToken);

            if (!proveedorExists)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["proveedorId"] = new[] { "El proveedor no existe." }
                    });
            }
        }

        var productIds = request.Items.Select(i => i.ProductoId).Distinct().ToList();
        var existingProductIds = await _dbContext.Productos.AsNoTracking()
            .Where(p => p.TenantId == tenantId && productIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (existingProductIds.Count != productIds.Count)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["items"] = new[] { "Uno o mas productos no existen." }
                });
        }

        var orden = new OrdenCompra(Guid.NewGuid(), tenantId, sucursalId, request.ProveedorId, nowUtc);
        _dbContext.OrdenesCompra.Add(orden);

        var items = request.Items.Select(item =>
            new OrdenCompraItem(
                Guid.NewGuid(),
                tenantId,
                orden.Id,
                item.ProductoId,
                item.Cantidad,
                nowUtc)).ToList();

        _dbContext.OrdenCompraItems.AddRange(items);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return orden.Id;
    }

    public async Task<IReadOnlyList<OrdenCompraListItemDto>> GetListAsync(
        Guid tenantId,
        Guid sucursalId,
        CancellationToken cancellationToken = default)
    {
        var query = from o in _dbContext.OrdenesCompra.AsNoTracking()
            where o.TenantId == tenantId && o.SucursalId == sucursalId
            join pr in _dbContext.Proveedores.AsNoTracking().Where(pr => pr.TenantId == tenantId)
                on o.ProveedorId equals pr.Id into prov
            from pr in prov.DefaultIfEmpty()
            select new OrdenCompraListItemDto(
                o.Id,
                o.ProveedorId,
                pr != null ? pr.Name : null,
                o.Estado.ToString().ToUpperInvariant(),
                o.CreatedAt,
                _dbContext.OrdenCompraItems.Count(i => i.TenantId == tenantId && i.OrdenCompraId == o.Id));

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<OrdenCompraDto?> GetByIdAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ordenCompraId,
        CancellationToken cancellationToken = default)
    {
        var orden = await (from o in _dbContext.OrdenesCompra.AsNoTracking()
                where o.TenantId == tenantId && o.SucursalId == sucursalId && o.Id == ordenCompraId
                join pr in _dbContext.Proveedores.AsNoTracking().Where(pr => pr.TenantId == tenantId)
                    on o.ProveedorId equals pr.Id into prov
                from pr in prov.DefaultIfEmpty()
                select new
                {
                    Orden = o,
                    Proveedor = pr != null ? pr.Name : null
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (orden is null)
        {
            return null;
        }

        var items = await (from i in _dbContext.OrdenCompraItems.AsNoTracking()
                join p in _dbContext.Productos.AsNoTracking() on i.ProductoId equals p.Id
                where i.TenantId == tenantId && i.OrdenCompraId == ordenCompraId && p.TenantId == tenantId
                orderby p.Name
                select new OrdenCompraItemDto(
                    i.Id,
                    i.ProductoId,
                    p.Name,
                    p.Sku,
                    i.Cantidad))
            .ToListAsync(cancellationToken);

        return new OrdenCompraDto(
            orden.Orden.Id,
            orden.Orden.SucursalId,
            orden.Orden.ProveedorId,
            orden.Proveedor,
            orden.Orden.Estado.ToString().ToUpperInvariant(),
            orden.Orden.CreatedAt,
            items);
    }

    public async Task<OrdenCompraDto> EnviarAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ordenCompraId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var orden = await _dbContext.OrdenesCompra
            .FirstOrDefaultAsync(o => o.TenantId == tenantId && o.SucursalId == sucursalId && o.Id == ordenCompraId, cancellationToken);

        if (orden is null)
        {
            throw new NotFoundException("Orden de compra no encontrada.");
        }

        if (orden.Estado != OrdenCompraEstado.Borrador)
        {
            throw new ConflictException("La orden no esta en borrador.");
        }

        orden.CambiarEstado(OrdenCompraEstado.Enviada, nowUtc);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(tenantId, sucursalId, ordenCompraId, cancellationToken)
               ?? throw new NotFoundException("Orden de compra no encontrada.");
    }

    public async Task<OrdenCompraDto> CancelarAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid ordenCompraId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var orden = await _dbContext.OrdenesCompra
            .FirstOrDefaultAsync(o => o.TenantId == tenantId && o.SucursalId == sucursalId && o.Id == ordenCompraId, cancellationToken);

        if (orden is null)
        {
            throw new NotFoundException("Orden de compra no encontrada.");
        }

        if (orden.Estado == OrdenCompraEstado.Recibida)
        {
            throw new ConflictException("La orden ya fue recibida.");
        }

        if (orden.Estado == OrdenCompraEstado.Cancelada)
        {
            throw new ConflictException("La orden ya esta cancelada.");
        }

        if (orden.Estado != OrdenCompraEstado.Borrador && orden.Estado != OrdenCompraEstado.Enviada)
        {
            throw new ConflictException("La orden no puede cancelarse en el estado actual.");
        }

        orden.CambiarEstado(OrdenCompraEstado.Cancelada, nowUtc);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(tenantId, sucursalId, ordenCompraId, cancellationToken)
               ?? throw new NotFoundException("Orden de compra no encontrada.");
    }
}
