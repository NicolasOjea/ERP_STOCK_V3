using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Application.DTOs.ListasPrecio;
using Pos.Application.UseCases.ListasPrecio;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/listas-precio")]
public sealed class ListasPrecioController : ControllerBase
{
    private readonly ListaPrecioService _listaPrecioService;

    public ListasPrecioController(ListaPrecioService listaPrecioService)
    {
        _listaPrecioService = listaPrecioService;
    }

    [HttpGet]
    [Authorize(Policy = "PERM_PRODUCTO_EDITAR")]
    [ProducesResponseType(typeof(IReadOnlyList<ListaPrecioDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ListaPrecioDto>>> GetList(CancellationToken cancellationToken)
    {
        var result = await _listaPrecioService.GetListAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "PERM_PRODUCTO_EDITAR")]
    [ProducesResponseType(typeof(ListaPrecioDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ListaPrecioDto>> Create(
        [FromBody] ListaPrecioCreateDto request,
        CancellationToken cancellationToken)
    {
        var created = await _listaPrecioService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetList), new { }, created);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Policy = "PERM_PRODUCTO_EDITAR")]
    [ProducesResponseType(typeof(ListaPrecioDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ListaPrecioDto>> Update(
        Guid id,
        [FromBody] ListaPrecioUpdateDto request,
        CancellationToken cancellationToken)
    {
        var updated = await _listaPrecioService.UpdateAsync(id, request, cancellationToken);
        return Ok(updated);
    }

    [HttpPut("{id:guid}/items")]
    [Authorize(Policy = "PERM_PRODUCTO_EDITAR")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpsertItems(
        Guid id,
        [FromBody] ListaPrecioItemsUpdateDto request,
        CancellationToken cancellationToken)
    {
        await _listaPrecioService.UpsertItemsAsync(id, request, cancellationToken);
        return NoContent();
    }
}
