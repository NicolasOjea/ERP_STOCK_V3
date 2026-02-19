using Pos.Application.DTOs.Comprobantes;

namespace Pos.Application.Abstractions;

public interface IComprobanteRepository
{
    Task<ComprobanteDto?> GetByIdAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid comprobanteId,
        CancellationToken cancellationToken = default);

    Task<ComprobanteDto> CreateBorradorAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid userId,
        Guid ventaId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);

    Task<ComprobanteDto> EmitirAsync(
        Guid tenantId,
        Guid sucursalId,
        Guid comprobanteId,
        FiscalEmitResultDto result,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);
}
