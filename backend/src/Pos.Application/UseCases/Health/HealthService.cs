using Pos.Application.Abstractions;
using Pos.Application.DTOs;

namespace Pos.Application.UseCases.Health;

public sealed class HealthService : IHealthService
{
    public Task<HealthStatusDto> GetAsync(CancellationToken cancellationToken = default)
    {
        var dto = new HealthStatusDto("ok", DateTimeOffset.UtcNow);
        return Task.FromResult(dto);
    }
}
