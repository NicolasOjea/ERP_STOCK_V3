using Pos.Domain.Enums;

namespace Pos.Application.Abstractions;

public interface IAuditLogService
{
    Task LogAsync(
        string entity,
        string entityId,
        AuditAction action,
        string? beforeJson,
        string? afterJson,
        string? metadataJson,
        CancellationToken cancellationToken = default);
}
