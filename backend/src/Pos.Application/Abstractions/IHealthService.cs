using Pos.Application.DTOs;

namespace Pos.Application.Abstractions;

public interface IHealthService
{
    Task<HealthStatusDto> GetAsync(CancellationToken cancellationToken = default);
}
