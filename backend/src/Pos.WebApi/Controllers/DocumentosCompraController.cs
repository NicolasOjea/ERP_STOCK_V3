using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Application.DTOs.DocumentosCompra;
using Pos.Application.UseCases.DocumentosCompra;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/documentos-compra")]
public sealed class DocumentosCompraController : ControllerBase
{
    private readonly DocumentoCompraService _documentoCompraService;

    public DocumentosCompraController(DocumentoCompraService documentoCompraService)
    {
        _documentoCompraService = documentoCompraService;
    }

    [HttpPost("parse")]
    [Authorize(Policy = "PERM_COMPRAS_REGISTRAR")]
    [ProducesResponseType(typeof(DocumentoCompraParseResultDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<DocumentoCompraParseResultDto>> Parse(
        [FromBody] JsonElement input,
        CancellationToken cancellationToken)
    {
        var result = await _documentoCompraService.ParseAsync(input, cancellationToken);
        return CreatedAtAction(nameof(Parse), new { }, result);
    }
}
