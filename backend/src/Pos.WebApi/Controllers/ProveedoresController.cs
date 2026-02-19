using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Application.DTOs.Proveedores;
using Pos.Application.UseCases.Proveedores;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/proveedores")]
public sealed class ProveedoresController : ControllerBase
{
    private readonly ProveedorService _proveedorService;

    public ProveedoresController(ProveedorService proveedorService)
    {
        _proveedorService = proveedorService;
    }

    [HttpGet]
    [Authorize(Policy = "PERM_PROVEEDOR_GESTIONAR")]
    [ProducesResponseType(typeof(IReadOnlyList<ProveedorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProveedorDto>>> Search(
        [FromQuery] string? search,
        [FromQuery] bool? activo,
        CancellationToken cancellationToken)
    {
        var results = await _proveedorService.SearchAsync(search, activo, cancellationToken);
        return Ok(results);
    }

    [HttpPost]
    [Authorize(Policy = "PERM_PROVEEDOR_GESTIONAR")]
    [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ProveedorDto>> Create(
        [FromBody] ProveedorCreateDto request,
        CancellationToken cancellationToken)
    {
        var created = await _proveedorService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Search), new { }, created);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Policy = "PERM_PROVEEDOR_GESTIONAR")]
    [ProducesResponseType(typeof(ProveedorDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProveedorDto>> Update(
        Guid id,
        [FromBody] ProveedorUpdateDto request,
        CancellationToken cancellationToken)
    {
        var updated = await _proveedorService.UpdateAsync(id, request, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "PERM_PROVEEDOR_GESTIONAR")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _proveedorService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}/delete-preview")]
    [Authorize(Policy = "PERM_PROVEEDOR_GESTIONAR")]
    [ProducesResponseType(typeof(ProveedorDeletePreviewDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProveedorDeletePreviewDto>> GetDeletePreview(
        Guid id,
        CancellationToken cancellationToken)
    {
        var preview = await _proveedorService.GetDeletePreviewAsync(id, cancellationToken);
        return Ok(preview);
    }

    [HttpPost("{id:guid}/delete")]
    [Authorize(Policy = "PERM_PROVEEDOR_GESTIONAR")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteWithProductResolution(
        Guid id,
        [FromBody] ProveedorDeleteRequestDto request,
        CancellationToken cancellationToken)
    {
        await _proveedorService.DeleteWithProductResolutionAsync(id, request, cancellationToken);
        return NoContent();
    }
}
