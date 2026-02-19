using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Application.DTOs.Importaciones;
using Pos.Application.UseCases.Importaciones;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/importaciones/productos")]
public sealed class ImportacionesController : ControllerBase
{
    private readonly ImportacionProductosService _service;

    public ImportacionesController(ImportacionProductosService service)
    {
        _service = service;
    }

    [HttpPost("preview")]
    [Authorize(Policy = "PERM_PRODUCTO_EDITAR")]
    [ProducesResponseType(typeof(ProductImportPreviewDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductImportPreviewDto>> Preview(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                ["file"] = new[] { "Debe adjuntar un archivo CSV." }
            })
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation error",
                Detail = "Archivo CSV requerido."
            });
        }

        using var reader = new StreamReader(file.OpenReadStream());
        var content = await reader.ReadToEndAsync(cancellationToken);
        var result = _service.Preview(content);
        return Ok(result);
    }

    [HttpPost("confirm")]
    [Authorize(Policy = "PERM_PRODUCTO_EDITAR")]
    [ProducesResponseType(typeof(ProductImportConfirmResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductImportConfirmResultDto>> Confirm(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                ["file"] = new[] { "Debe adjuntar un archivo CSV." }
            })
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation error",
                Detail = "Archivo CSV requerido."
            });
        }

        using var reader = new StreamReader(file.OpenReadStream());
        var content = await reader.ReadToEndAsync(cancellationToken);
        var result = await _service.ConfirmAsync(content, cancellationToken);
        return Ok(result);
    }
}
