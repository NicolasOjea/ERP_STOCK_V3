using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Application.DTOs.Auditoria;
using Pos.Application.UseCases.Auditoria;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/auditoria")]
public sealed class AuditoriaController : ControllerBase
{
    private readonly AuditoriaService _service;

    public AuditoriaController(AuditoriaService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = "PERM_REPORTES_VER")]
    [ProducesResponseType(typeof(AuditLogQueryResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuditLogQueryResultDto>> Get(
        [FromQuery] string? entidad,
        [FromQuery] Guid? usuarioId,
        [FromQuery] DateTimeOffset? desde,
        [FromQuery] DateTimeOffset? hasta,
        [FromQuery] int? page,
        [FromQuery] int? size,
        CancellationToken cancellationToken)
    {
        var result = await _service.SearchAsync(entidad, usuarioId, desde, hasta, page, size, cancellationToken);
        return Ok(result);
    }
}
