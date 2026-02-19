using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Application.DTOs.Comprobantes;
using Pos.Application.UseCases.Comprobantes;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/comprobantes")]
public sealed class ComprobantesController : ControllerBase
{
    private readonly ComprobantesService _service;

    public ComprobantesController(ComprobantesService service)
    {
        _service = service;
    }

    [HttpPost("borrador-desde-venta/{ventaId:guid}")]
    [Authorize(Policy = "PERM_VENTA_CONFIRMAR")]
    [ProducesResponseType(typeof(ComprobanteDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ComprobanteDto>> CrearDesdeVenta(
        Guid ventaId,
        CancellationToken cancellationToken)
    {
        var result = await _service.CrearBorradorDesdeVentaAsync(ventaId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("{id:guid}/emitir")]
    [Authorize(Policy = "PERM_VENTA_CONFIRMAR")]
    [ProducesResponseType(typeof(ComprobanteDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ComprobanteDto>> Emitir(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _service.EmitirAsync(id, cancellationToken);
        return Ok(result);
    }
}
