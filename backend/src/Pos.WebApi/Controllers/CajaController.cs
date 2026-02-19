using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Application.DTOs.Caja;
using Pos.Application.UseCases.Caja;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/caja")]
public sealed class CajaController : ControllerBase
{
    private readonly CajaService _cajaService;

    public CajaController(CajaService cajaService)
    {
        _cajaService = cajaService;
    }

    [HttpGet]
    [Authorize(Policy = "ROLE_ENCARGADO_ADMIN")]
    [ProducesResponseType(typeof(IReadOnlyList<CajaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CajaDto>>> GetCajas(
        [FromQuery] bool? activo,
        CancellationToken cancellationToken)
    {
        var cajas = await _cajaService.GetCajasAsync(activo, cancellationToken);
        return Ok(cajas);
    }

    [HttpPost]
    [Authorize(Policy = "ROLE_ENCARGADO_ADMIN")]
    [ProducesResponseType(typeof(CajaDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CajaDto>> CreateCaja(
        [FromBody] CajaCreateDto request,
        CancellationToken cancellationToken)
    {
        var created = await _cajaService.CreateCajaAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetCajas), new { }, created);
    }

    [HttpPost("sesiones/abrir")]
    [Authorize(Policy = "ROLE_ENCARGADO_ADMIN")]
    [ProducesResponseType(typeof(CajaSesionDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CajaSesionDto>> AbrirSesion(
        [FromBody] CajaSesionAbrirDto request,
        CancellationToken cancellationToken)
    {
        var session = await _cajaService.AbrirSesionAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetResumen), new { id = session.Id }, session);
    }

    [HttpPost("sesiones/{id:guid}/movimientos")]
    [Authorize(Policy = "PERM_CAJA_MOVIMIENTO")]
    [ProducesResponseType(typeof(CajaMovimientoDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CajaMovimientoDto>> RegistrarMovimiento(
        Guid id,
        [FromBody] CajaMovimientoCreateDto request,
        CancellationToken cancellationToken)
    {
        var movimiento = await _cajaService.RegistrarMovimientoAsync(id, request, cancellationToken);
        return CreatedAtAction(nameof(GetResumen), new { id }, movimiento);
    }

    [HttpPost("sesiones/{id:guid}/cerrar")]
    [Authorize(Policy = "ROLE_ENCARGADO_ADMIN")]
    [ProducesResponseType(typeof(CajaCierreResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CajaCierreResultDto>> CerrarSesion(
        Guid id,
        [FromBody] CajaCierreRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _cajaService.CerrarSesionAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("sesiones/{id:guid}/resumen")]
    [Authorize(Policy = "PERM_CAJA_MOVIMIENTO")]
    [ProducesResponseType(typeof(CajaResumenDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CajaResumenDto>> GetResumen(Guid id, CancellationToken cancellationToken)
    {
        var resumen = await _cajaService.GetResumenAsync(id, cancellationToken);
        return Ok(resumen);
    }

    [HttpGet("sesiones/historial")]
    [Authorize(Policy = "ROLE_ENCARGADO_ADMIN")]
    [ProducesResponseType(typeof(IReadOnlyList<CajaHistorialDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CajaHistorialDto>>> GetHistorial(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        CancellationToken cancellationToken)
    {
        var result = await _cajaService.GetHistorialAsync(from, to, cancellationToken);
        return Ok(result);
    }
}
