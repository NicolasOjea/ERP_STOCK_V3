using Pos.Application.DTOs.Recepciones;

namespace Pos.Application.Abstractions;

public interface IRecepcionRepository
{
    Task<RecepcionConfirmResultDto> ConfirmarAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid preRecepcionId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);
}
