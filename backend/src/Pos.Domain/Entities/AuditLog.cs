using Pos.Domain.Common;
using Pos.Domain.Enums;

namespace Pos.Domain.Entities;

public sealed class AuditLog : EntityBase
{
    private AuditLog()
    {
    }

    public AuditLog(
        Guid id,
        Guid tenantId,
        Guid sucursalId,
        Guid? userId,
        AuditAction action,
        string entityName,
        string entityId,
        DateTimeOffset occurredAtUtc,
        string? beforeJson,
        string? afterJson,
        string? metadataJson)
        : base(id, tenantId, occurredAtUtc)
    {
        if (sucursalId == Guid.Empty) throw new ArgumentException("SucursalId cannot be empty.", nameof(sucursalId));

        SucursalId = sucursalId;
        UserId = userId;
        Action = action;
        EntityName = entityName;
        EntityId = entityId;
        OccurredAt = occurredAtUtc;
        BeforeJson = beforeJson;
        AfterJson = afterJson;
        MetadataJson = metadataJson;
    }

    public Guid SucursalId { get; private set; }
    public Guid? UserId { get; private set; }
    public AuditAction Action { get; private set; }
    public string EntityName { get; private set; }
    public string EntityId { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string? BeforeJson { get; private set; }
    public string? AfterJson { get; private set; }
    public string? MetadataJson { get; private set; }
}
