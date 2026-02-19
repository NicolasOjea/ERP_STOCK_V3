using Pos.Application.Abstractions;
using Pos.Domain.Entities;
using Pos.Domain.Enums;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Services;

public sealed class AuditLogService : IAuditLogService
{
    private readonly PosDbContext _dbContext;
    private readonly IRequestContext _requestContext;

    public AuditLogService(PosDbContext dbContext, IRequestContext requestContext)
    {
        _dbContext = dbContext;
        _requestContext = requestContext;
    }

    public async Task LogAsync(
        string entity,
        string entityId,
        AuditAction action,
        string? beforeJson,
        string? afterJson,
        string? metadataJson,
        CancellationToken cancellationToken = default)
    {
        if (_requestContext.TenantId == Guid.Empty || _requestContext.SucursalId == Guid.Empty)
        {
            throw new InvalidOperationException("Request context is missing tenant or sucursal.");
        }

        var now = DateTimeOffset.UtcNow;
        Guid? userId = _requestContext.UserId == Guid.Empty ? (Guid?)null : _requestContext.UserId;

        var audit = new AuditLog(
            Guid.NewGuid(),
            _requestContext.TenantId,
            _requestContext.SucursalId,
            userId,
            action,
            entity,
            entityId,
            now,
            beforeJson,
            afterJson,
            metadataJson);

        _dbContext.AuditLogs.Add(audit);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
