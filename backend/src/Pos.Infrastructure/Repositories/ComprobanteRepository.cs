using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Comprobantes;
using Pos.Domain.Entities;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class ComprobanteRepository : IComprobanteRepository
{
    private readonly PosDbContext _dbContext;

    public ComprobanteRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ComprobanteDto?> GetByIdAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid comprobanteId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Comprobantes.AsNoTracking()
            .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.SucursalId == sucursalId && c.Id == comprobanteId, cancellationToken);

        return entity is null ? null : Map(entity);
    }

    public async Task<ComprobanteDto> CreateBorradorAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid userId,
        Guid ventaId,
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

        var exists = await _dbContext.Comprobantes.AsNoTracking()
            .AnyAsync(c => c.TenantId == tenantId && c.VentaId == ventaId, cancellationToken);

        if (exists)
        {
            throw new ConflictException("Comprobante ya existe para la venta.");
        }

        var comprobante = new Comprobante(
            Guid.NewGuid(),
            tenantId,
            sucursalId,
            ventaId,
            userId,
            venta.TotalNeto,
            nowUtc);

        _dbContext.Comprobantes.Add(comprobante);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Map(comprobante);
    }

    public async Task<ComprobanteDto> EmitirAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid comprobanteId,
        FiscalEmitResultDto result,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var comprobante = await _dbContext.Comprobantes
            .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.SucursalId == sucursalId && c.Id == comprobanteId, cancellationToken);

        if (comprobante is null)
        {
            throw new NotFoundException("Comprobante no encontrado.");
        }

        if (comprobante.Estado != ComprobanteEstado.Borrador)
        {
            throw new ConflictException("El comprobante no esta en borrador.");
        }

        comprobante.Emitir(result.Provider, result.Numero, result.Payload, result.EmitidoAt, nowUtc);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(comprobante);
    }

    private static ComprobanteDto Map(Comprobante entity)
    {
        return new ComprobanteDto(
            entity.Id,
            entity.VentaId,
            entity.Estado.ToString().ToUpperInvariant(),
            entity.Total,
            entity.Numero,
            entity.FiscalProvider,
            entity.EmitidoAt,
            entity.CreatedAt);
    }
}
