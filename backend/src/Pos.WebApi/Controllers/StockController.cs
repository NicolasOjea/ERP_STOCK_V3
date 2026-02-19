using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Application.DTOs.Stock;
using Pos.Application.UseCases.Stock;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/stock")]
public sealed class StockController : ControllerBase
{
    private readonly StockService _stockService;

    public StockController(StockService stockService)
    {
        _stockService = stockService;
    }

    [Authorize(Policy = "PERM_STOCK_AJUSTAR")]
    [HttpPost("adjust")]
    public ActionResult<object> Adjust([FromBody] StockAdjustRequest request)
    {
        return Ok(new { status = "ok" });
    }

    [Authorize(Policy = "PERM_STOCK_AJUSTAR")]
    [HttpPost("ajustes")]
    [ProducesResponseType(typeof(StockMovimientoDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<StockMovimientoDto>> RegistrarAjuste(
        [FromBody] StockMovimientoCreateDto request,
        CancellationToken cancellationToken)
    {
        var movimiento = await _stockService.RegistrarMovimientoAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetMovimientos), new { }, movimiento);
    }

    [Authorize(Policy = "PERM_STOCK_AJUSTAR")]
    [HttpGet("saldos")]
    [ProducesResponseType(typeof(IReadOnlyList<StockSaldoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<StockSaldoDto>>> GetSaldos(
        [FromQuery] string? search,
        [FromQuery] Guid? proveedorId,
        CancellationToken cancellationToken)
    {
        var result = await _stockService.GetSaldosAsync(search, proveedorId, cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = "PERM_STOCK_AJUSTAR")]
    [HttpGet("movimientos")]
    [ProducesResponseType(typeof(IReadOnlyList<StockMovimientoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<StockMovimientoDto>>> GetMovimientos(
        [FromQuery] Guid? productoId,
        [FromQuery] long? ventaNumero,
        [FromQuery] bool? facturada,
        [FromQuery] DateTimeOffset? desde,
        [FromQuery] DateTimeOffset? hasta,
        CancellationToken cancellationToken)
    {
        var result = await _stockService.GetMovimientosAsync(productoId, ventaNumero, facturada, desde, hasta, cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = "PERM_STOCK_AJUSTAR")]
    [HttpGet("alertas")]
    [ProducesResponseType(typeof(IReadOnlyList<StockAlertaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<StockAlertaDto>>> GetAlertas(
        [FromQuery] Guid? proveedorId,
        CancellationToken cancellationToken)
    {
        var result = await _stockService.GetAlertasAsync(proveedorId, cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = "PERM_STOCK_AJUSTAR")]
    [HttpPost("sugerido-compra")]
    [ProducesResponseType(typeof(StockSugeridoCompraDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<StockSugeridoCompraDto>> GetSugeridoCompra(CancellationToken cancellationToken)
    {
        var result = await _stockService.GetSugeridoCompraAsync(cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = "PERM_STOCK_AJUSTAR")]
    [HttpPost("alertas/remito")]
    public async Task<IActionResult> GenerarRemitoAlertas(
        [FromBody] StockRemitoRequestDto request,
        CancellationToken cancellationToken)
    {
        var pdf = await _stockService.GenerarRemitoAlertasAsync(request, cancellationToken);
        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        return File(pdf, "application/pdf", "remito-alertas.pdf");
    }
}

public sealed record StockAdjustRequest(Guid ProductId, decimal Quantity);
