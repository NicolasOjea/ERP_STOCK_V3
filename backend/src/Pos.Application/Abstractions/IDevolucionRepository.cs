using Pos.Application.DTOs.Devoluciones;

namespace Pos.Application.Abstractions;

public interface IDevolucionRepository
{
    Task<DevolucionResultDto> CreateAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid userId,
        DevolucionCreateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);
}
