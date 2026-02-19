using Pos.Application.DTOs.PreRecepciones;

namespace Pos.Application.Abstractions;

public interface IPreRecepcionRepository
{
    Task<Guid> CreateAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid documentoCompraId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<PreRecepcionDto?> GetByIdAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid preRecepcionId,
        CancellationToken cancellationToken = default);

    Task<PreRecepcionDto> UpdateAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid preRecepcionId,
        PreRecepcionUpdateDto request,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);
}
