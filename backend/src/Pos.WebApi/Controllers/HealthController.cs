using Microsoft.AspNetCore.Mvc;
using Pos.Application.Abstractions;
using Pos.Application.DTOs;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/health")]
public sealed class HealthController : ControllerBase
{
    private readonly IHealthService _healthService;

    public HealthController(IHealthService healthService)
    {
        _healthService = healthService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(HealthStatusDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HealthStatusDto>> Get(CancellationToken cancellationToken)
    {
        var result = await _healthService.GetAsync(cancellationToken);
        return Ok(result);
    }
}
