using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Application.DTOs.Ventas;
using Pos.Application.UseCases.Ventas;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/ventas")]
public sealed class VentasController : ControllerBase
{
    private readonly VentaService _ventaService;

    public VentasController(VentaService ventaService)
    {
        _ventaService = ventaService;
    }

    [HttpPost]
    [Authorize(Policy = "PERM_VENTA_CREAR")]
    [ProducesResponseType(typeof(VentaDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<VentaDto>> IniciarVenta(CancellationToken cancellationToken)
    {
        var venta = await _ventaService.IniciarVentaAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = venta.Id }, venta);
    }

    [HttpPost("{id:guid}/items/scan")]
    [Authorize(Policy = "PERM_VENTA_CREAR")]
    [ProducesResponseType(typeof(VentaItemDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<VentaItemDto>> ScanItem(
        Guid id,
        [FromBody] VentaScanRequestDto request,
        CancellationToken cancellationToken)
    {
        var item = await _ventaService.AgregarItemPorCodigoAsync(id, request, cancellationToken);
        return Ok(item);
    }

    [HttpPost("{id:guid}/items")]
    [Authorize(Policy = "PERM_VENTA_CREAR")]
    [ProducesResponseType(typeof(VentaItemDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<VentaItemDto>> AddItem(
        Guid id,
        [FromBody] VentaItemByProductRequestDto request,
        CancellationToken cancellationToken)
    {
        var item = await _ventaService.AgregarItemPorProductoAsync(id, request, cancellationToken);
        return Ok(item);
    }

    [HttpPatch("{id:guid}/items/{itemId:guid}")]
    [Authorize(Policy = "PERM_VENTA_CREAR")]
    [ProducesResponseType(typeof(VentaItemDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<VentaItemDto>> UpdateItem(
        Guid id,
        Guid itemId,
        [FromBody] VentaItemUpdateDto request,
        CancellationToken cancellationToken)
    {
        var item = await _ventaService.ActualizarItemAsync(id, itemId, request, cancellationToken);
        return Ok(item);
    }

    [HttpDelete("{id:guid}/items/{itemId:guid}")]
    [Authorize(Policy = "PERM_VENTA_CREAR")]
    [ProducesResponseType(typeof(VentaItemDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<VentaItemDto>> RemoveItem(
        Guid id,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        var item = await _ventaService.QuitarItemAsync(id, itemId, cancellationToken);
        return Ok(item);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "PERM_VENTA_CREAR")]
    [ProducesResponseType(typeof(VentaDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<VentaDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var venta = await _ventaService.GetByIdAsync(id, cancellationToken);
        return Ok(venta);
    }

    [HttpGet("numero/{numero:long}/ticket")]
    [Authorize(Policy = "PERM_VENTA_CREAR")]
    [ProducesResponseType(typeof(VentaTicketDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<VentaTicketDto>> GetTicketByNumero(
        long numero,
        CancellationToken cancellationToken)
    {
        var ticket = await _ventaService.GetTicketByNumeroAsync(numero, cancellationToken);
        return Ok(ticket);
    }

    [HttpPost("{id:guid}/confirmar")]
    [Authorize(Policy = "PERM_VENTA_CONFIRMAR")]
    [ProducesResponseType(typeof(VentaConfirmResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<VentaConfirmResultDto>> Confirmar(
        Guid id,
        [FromBody] VentaConfirmRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _ventaService.ConfirmarVentaAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/anular")]
    [Authorize(Policy = "PERM_VENTA_ANULAR")]
    [ProducesResponseType(typeof(VentaAnularResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<VentaAnularResultDto>> Anular(
        Guid id,
        [FromBody] VentaAnularRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _ventaService.AnularVentaAsync(id, request, cancellationToken);
        return Ok(result);
    }
}
