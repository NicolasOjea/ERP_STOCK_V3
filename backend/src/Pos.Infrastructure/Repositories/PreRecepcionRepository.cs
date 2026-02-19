using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.PreRecepciones;
using Pos.Domain.Entities;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class PreRecepcionRepository : IPreRecepcionRepository
{
    private readonly PosDbContext _dbContext;

    public PreRecepcionRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> CreateAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid documentoCompraId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var documento = await _dbContext.DocumentosCompra.AsNoTracking()
            .FirstOrDefaultAsync(d => d.TenantId == tenantId && d.SucursalId == sucursalId && d.Id == documentoCompraId, cancellationToken);

        if (documento is null)
        {
            throw new NotFoundException("Documento de compra no encontrado.");
        }

        var docItems = await _dbContext.DocumentoCompraItems.AsNoTracking()
            .Where(i => i.TenantId == tenantId && i.DocumentoCompraId == documentoCompraId)
            .OrderBy(i => i.Id)
            .ToListAsync(cancellationToken);

        if (docItems.Count == 0)
        {
            throw new ConflictException("El documento no tiene items.");
        }

        var codes = docItems.Select(i => i.Codigo.Trim()).Distinct().ToList();

        var codeMatches = await (from c in _dbContext.ProductoCodigos.AsNoTracking()
                join p in _dbContext.Productos.AsNoTracking() on c.ProductoId equals p.Id
                where c.TenantId == tenantId
                      && p.TenantId == tenantId
                      && codes.Contains(c.Codigo)
                select new
                {
                    c.Codigo,
                    ProductoId = p.Id,
                    p.Name,
                    p.Sku
                })
            .ToListAsync(cancellationToken);

        var codeMatchMap = codeMatches
            .GroupBy(x => x.Codigo)
            .ToDictionary(g => g.Key, g => g.First());

        var unmatchedCodes = codes.Where(c => !codeMatchMap.ContainsKey(c)).ToList();

        var skuMatches = await _dbContext.Productos.AsNoTracking()
            .Where(p => p.TenantId == tenantId && unmatchedCodes.Contains(p.Sku))
            .Select(p => new { p.Sku, ProductoId = p.Id, p.Name })
            .ToListAsync(cancellationToken);

        var skuMatchMap = skuMatches
            .GroupBy(x => x.Sku)
            .ToDictionary(g => g.Key, g => g.First());

        var preRecepcion = new PreRecepcion(Guid.NewGuid(), tenantId, sucursalId, documentoCompraId, nowUtc);
        _dbContext.PreRecepciones.Add(preRecepcion);

        var preItems = new List<PreRecepcionItem>();
        foreach (var item in docItems)
        {
            var codigo = item.Codigo.Trim();
            Guid? productoId = null;
            var estado = PreRecepcionItemEstado.NoEncontrado;

            if (codeMatchMap.TryGetValue(codigo, out var matched))
            {
                productoId = matched.ProductoId;
                estado = PreRecepcionItemEstado.Ok;
            }
            else if (skuMatchMap.TryGetValue(codigo, out var skuMatched))
            {
                productoId = skuMatched.ProductoId;
                estado = PreRecepcionItemEstado.Duda;
            }

            preItems.Add(new PreRecepcionItem(
                Guid.NewGuid(),
                tenantId,
                preRecepcion.Id,
                item.Id,
                item.Codigo,
                item.Descripcion,
                item.Cantidad,
                item.CostoUnitario,
                productoId,
                estado,
                nowUtc));
        }

        _dbContext.PreRecepcionItems.AddRange(preItems);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return preRecepcion.Id;
    }

    public async Task<PreRecepcionDto?> GetByIdAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid preRecepcionId,
        CancellationToken cancellationToken = default)
    {
        var preRecepcion = await _dbContext.PreRecepciones.AsNoTracking()
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.SucursalId == sucursalId && p.Id == preRecepcionId, cancellationToken);

        if (preRecepcion is null)
        {
            return null;
        }

        var items = await (from i in _dbContext.PreRecepcionItems.AsNoTracking()
                join p in _dbContext.Productos.AsNoTracking().Where(p => p.TenantId == tenantId)
                    on i.ProductoId equals p.Id into prod
                from p in prod.DefaultIfEmpty()
                where i.TenantId == tenantId && i.PreRecepcionId == preRecepcionId
                orderby i.Codigo
                select new PreRecepcionItemDto(
                    i.Id,
                    i.Codigo,
                    i.Descripcion,
                    i.Cantidad,
                    i.CostoUnitario,
                    ToEstadoString(i.Estado),
                    i.ProductoId,
                    p != null ? p.Name : null,
                    p != null ? p.Sku : null))
            .ToListAsync(cancellationToken);

        return new PreRecepcionDto(
            preRecepcion.Id,
            preRecepcion.DocumentoCompraId,
            preRecepcion.CreatedAt,
            items);
    }

    public async Task<PreRecepcionDto> UpdateAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid preRecepcionId,
        PreRecepcionUpdateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var preRecepcion = await _dbContext.PreRecepciones
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.SucursalId == sucursalId && p.Id == preRecepcionId, cancellationToken);

        if (preRecepcion is null)
        {
            throw new NotFoundException("Pre-recepcion no encontrada.");
        }

        var itemIds = request.Items.Select(i => i.ItemId).Distinct().ToList();
        var items = await _dbContext.PreRecepcionItems
            .Where(i => i.TenantId == tenantId && i.PreRecepcionId == preRecepcionId && itemIds.Contains(i.Id))
            .ToListAsync(cancellationToken);

        if (items.Count != itemIds.Count)
        {
            throw new NotFoundException("Item de pre-recepcion no encontrado.");
        }

        var productIdsToValidate = request.Items
            .Where(i => i.ProductoId.HasValue)
            .Select(i => i.ProductoId!.Value)
            .Distinct()
            .ToList();

        if (productIdsToValidate.Count > 0)
        {
            var existing = await _dbContext.Productos.AsNoTracking()
                .Where(p => p.TenantId == tenantId && productIdsToValidate.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            if (existing.Count != productIdsToValidate.Count)
            {
                throw new ValidationException(
                    "Validacion fallida.",
                    new Dictionary<string, string[]>
                    {
                        ["productoId"] = new[] { "Producto no existe." }
                    });
            }
        }

        foreach (var update in request.Items)
        {
            var entity = items.Single(i => i.Id == update.ItemId);

            if (update.Cantidad.HasValue && update.Cantidad.Value > 0)
            {
                entity.ActualizarCantidad(update.Cantidad.Value, nowUtc);
            }

            if (update.ProductoId.HasValue)
            {
                entity.AsignarProducto(update.ProductoId.Value, PreRecepcionItemEstado.Ok, nowUtc);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return await GetByIdAsync(tenantId, sucursalId, preRecepcionId, cancellationToken)
               ?? throw new NotFoundException("Pre-recepcion no encontrada.");
    }

    private static string ToEstadoString(PreRecepcionItemEstado estado)
    {
        return estado switch
        {
            PreRecepcionItemEstado.Ok => "OK",
            PreRecepcionItemEstado.Duda => "DUDA",
            PreRecepcionItemEstado.NoEncontrado => "NO_ENCONTRADO",
            _ => "NO_ENCONTRADO"
        };
    }
}
