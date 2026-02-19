using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Application.DTOs.Categorias;
using Pos.Application.UseCases.Categorias;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/categorias-precio")]
public sealed class CategoriasPrecioController : ControllerBase
{
    private readonly CategoriaPrecioService _categoriaService;

    public CategoriasPrecioController(CategoriaPrecioService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    [HttpGet]
    [Authorize(Policy = "PERM_PRODUCTO_VER")]
    [ProducesResponseType(typeof(IReadOnlyList<CategoriaPrecioDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoriaPrecioDto>>> Search(
        [FromQuery] string? search,
        [FromQuery] bool? activo,
        CancellationToken cancellationToken)
    {
        var result = await _categoriaService.SearchAsync(search, activo, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "PERM_PRODUCTO_EDITAR")]
    [ProducesResponseType(typeof(CategoriaPrecioDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CategoriaPrecioDto>> Create(
        [FromBody] CategoriaPrecioCreateDto request,
        CancellationToken cancellationToken)
    {
        var created = await _categoriaService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Search), new { }, created);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Policy = "PERM_PRODUCTO_EDITAR")]
    [ProducesResponseType(typeof(CategoriaPrecioDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CategoriaPrecioDto>> Update(
        Guid id,
        [FromBody] CategoriaPrecioUpdateDto request,
        CancellationToken cancellationToken)
    {
        var updated = await _categoriaService.UpdateAsync(id, request, cancellationToken);
        return Ok(updated);
    }
}
