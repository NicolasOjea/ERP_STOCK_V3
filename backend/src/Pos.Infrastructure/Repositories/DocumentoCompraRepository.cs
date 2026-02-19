using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.DocumentosCompra;
using Pos.Domain.Entities;
using Pos.Domain.Exceptions;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class DocumentoCompraRepository : IDocumentoCompraRepository
{
    private readonly PosDbContext _dbContext;

    public DocumentoCompraRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> CreateAsync(
        Guid tenantId,
        Guid sucursalId,
        ParsedDocumentDto parsed,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        if (parsed.ProveedorId.HasValue)
        {
            var proveedorExists = await _dbContext.Proveedores.AsNoTracking()
                .AnyAsync(p => p.TenantId == tenantId && p.Id == parsed.ProveedorId.Value, cancellationToken);

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

        var documento = new DocumentoCompra(
            Guid.NewGuid(),
            tenantId,
            sucursalId,
            parsed.ProveedorId,
            parsed.Numero,
            parsed.Fecha,
            nowUtc);

        _dbContext.DocumentosCompra.Add(documento);

        var items = parsed.Items.Select(i => new DocumentoCompraItem(
            Guid.NewGuid(),
            tenantId,
            documento.Id,
            i.Codigo,
            i.Descripcion,
            i.Cantidad,
            i.CostoUnitario,
            nowUtc)).ToList();

        _dbContext.DocumentoCompraItems.AddRange(items);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return documento.Id;
    }

    public async Task<DocumentoCompraDto?> GetByIdAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid documentoCompraId,
        CancellationToken cancellationToken = default)
    {
        var documento = await _dbContext.DocumentosCompra.AsNoTracking()
            .FirstOrDefaultAsync(d => d.TenantId == tenantId && d.SucursalId == sucursalId && d.Id == documentoCompraId, cancellationToken);

        if (documento is null)
        {
            return null;
        }

        var items = await _dbContext.DocumentoCompraItems.AsNoTracking()
            .Where(i => i.TenantId == tenantId && i.DocumentoCompraId == documento.Id)
            .OrderBy(i => i.Codigo)
            .Select(i => new DocumentoCompraItemDto(
                i.Id,
                i.Codigo,
                i.Descripcion,
                i.Cantidad,
                i.CostoUnitario))
            .ToListAsync(cancellationToken);

        return new DocumentoCompraDto(
            documento.Id,
            documento.ProveedorId,
            documento.Numero,
            documento.Fecha,
            documento.CreatedAt,
            items);
    }
}
