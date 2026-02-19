using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Auditoria;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class AuditLogQueryRepository : IAuditLogQueryRepository
{
    private readonly PosDbContext _dbContext;

    public AuditLogQueryRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AuditLogQueryResultDto> SearchAsync(
        Guid tenantId,
        Guid sucursalId,
        AuditLogQueryRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.AuditLogs.AsNoTracking()
            .Where(a => a.TenantId == tenantId && a.SucursalId == sucursalId);

        if (!string.IsNullOrWhiteSpace(request.Entidad))
        {
            var entidad = request.Entidad.Trim();
            query = query.Where(a => EF.Functions.ILike(a.EntityName, entidad));
        }

        if (request.UsuarioId.HasValue)
        {
            query = query.Where(a => a.UserId == request.UsuarioId.Value);
        }

        if (request.Desde.HasValue)
        {
            query = query.Where(a => a.OccurredAt >= request.Desde.Value);
        }

        if (request.Hasta.HasValue)
        {
            query = query.Where(a => a.OccurredAt <= request.Hasta.Value);
        }

        var total = await query.CountAsync(cancellationToken);

        var skip = (request.Page - 1) * request.Size;
        var items = await query
            .OrderByDescending(a => a.OccurredAt)
            .Skip(skip)
            .Take(request.Size)
            .Select(a => new AuditLogListItemDto(
                a.Id,
                a.EntityName,
                a.EntityId,
                a.Action.ToString().ToUpperInvariant(),
                a.UserId,
                a.OccurredAt,
                a.MetadataJson))
            .ToListAsync(cancellationToken);

        return new AuditLogQueryResultDto(items, request.Page, request.Size, total);
    }
}
