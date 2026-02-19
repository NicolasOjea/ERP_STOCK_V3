using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Application.DTOs.Devoluciones;
using Pos.Application.UseCases.Devoluciones;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/devoluciones")]
public sealed class DevolucionesController : ControllerBase
{
    private readonly DevolucionService _devolucionService;

    public DevolucionesController(DevolucionService devolucionService)
    {
        _devolucionService = devolucionService;
    }

    [HttpPost]
    [Authorize(Policy = "PERM_DEVOLUCION_REGISTRAR")]
    [ProducesResponseType(typeof(DevolucionResultDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<DevolucionResultDto>> Crear(
        [FromBody] DevolucionCreateDto request,
        CancellationToken cancellationToken)
    {
        var result = await _devolucionService.CrearAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
