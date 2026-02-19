using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Application.DTOs.PreRecepciones;
using Pos.Application.DTOs.Recepciones;
using Pos.Application.UseCases.PreRecepciones;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/pre-recepciones")]
public sealed class PreRecepcionesController : ControllerBase
{
    private readonly PreRecepcionService _preRecepcionService;

    public PreRecepcionesController(PreRecepcionService preRecepcionService)
    {
        _preRecepcionService = preRecepcionService;
    }

    [HttpPost]
    [Authorize(Policy = "PERM_COMPRAS_REGISTRAR")]
    [ProducesResponseType(typeof(PreRecepcionDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<PreRecepcionDto>> Create(
        [FromBody] PreRecepcionCreateDto request,
        CancellationToken cancellationToken)
    {
        var created = await _preRecepcionService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "PERM_COMPRAS_REGISTRAR")]
    [ProducesResponseType(typeof(PreRecepcionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PreRecepcionDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var preRecepcion = await _preRecepcionService.GetByIdAsync(id, cancellationToken);
        return Ok(preRecepcion);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Policy = "PERM_COMPRAS_REGISTRAR")]
    [ProducesResponseType(typeof(PreRecepcionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PreRecepcionDto>> Update(
        Guid id,
        [FromBody] PreRecepcionUpdateDto request,
        CancellationToken cancellationToken)
    {
        var updated = await _preRecepcionService.UpdateAsync(id, request, cancellationToken);
        return Ok(updated);
    }

    [HttpPost("{id:guid}/confirmar")]
    [Authorize(Policy = "PERM_COMPRAS_REGISTRAR")]
    [ProducesResponseType(typeof(RecepcionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<RecepcionDto>> Confirmar(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _preRecepcionService.ConfirmarAsync(id, cancellationToken);
        return Ok(result.Recepcion);
    }
}
