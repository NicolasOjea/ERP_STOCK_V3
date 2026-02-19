using Pos.Application.DTOs.Pricing;

namespace Pos.Application.Abstractions;

public interface IPromocionRepository
{
    Task<IReadOnlyList<PromocionAplicableDto>> GetActivasAsync(
        Guid tenantId,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default);
}
