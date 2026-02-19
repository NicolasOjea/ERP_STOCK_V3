using Pos.Application.DTOs.Auditoria;

namespace Pos.Application.Abstractions;

public interface IAuditLogQueryRepository
{
    Task<AuditLogQueryResultDto> SearchAsync(
        Guid tenantId,
        Guid sucursalId,
        AuditLogQueryRequestDto request,
        CancellationToken cancellationToken = default);
}
