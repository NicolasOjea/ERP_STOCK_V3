using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Application.DTOs.Reportes;
using Pos.Application.UseCases.Reportes;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/reportes")]
public sealed class ReportesController : ControllerBase
{
    private readonly ReportesService _service;

    public ReportesController(ReportesService service)
    {
        _service = service;
    }

    [HttpGet("resumen-ventas")]
    [Authorize(Policy = "PERM_REPORTES_VER")]
    [ProducesResponseType(typeof(ReportResumenVentasDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReportResumenVentasDto>> ResumenVentas(
        [FromQuery] DateTimeOffset? desde,
        [FromQuery] DateTimeOffset? hasta,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetResumenVentasAsync(desde, hasta, cancellationToken);
        return Ok(result);
    }

    [HttpGet("ventas-por-dia")]
    [Authorize(Policy = "PERM_REPORTES_VER")]
    [ProducesResponseType(typeof(ReportChartDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReportChartDto>> VentasPorDia(
        [FromQuery] DateTimeOffset? desde,
        [FromQuery] DateTimeOffset? hasta,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetVentasPorDiaAsync(desde, hasta, cancellationToken);
        return Ok(result);
    }

    [HttpGet("medios-pago")]
    [Authorize(Policy = "PERM_REPORTES_VER")]
    [ProducesResponseType(typeof(ReportChartDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReportChartDto>> MediosPago(
        [FromQuery] DateTimeOffset? desde,
        [FromQuery] DateTimeOffset? hasta,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetMediosPagoAsync(desde, hasta, cancellationToken);
        return Ok(result);
    }

    [HttpGet("top-productos")]
    [Authorize(Policy = "PERM_REPORTES_VER")]
    [ProducesResponseType(typeof(ReportTableDto<TopProductoItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReportTableDto<TopProductoItemDto>>> TopProductos(
        [FromQuery] DateTimeOffset? desde,
        [FromQuery] DateTimeOffset? hasta,
        [FromQuery] int? top,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetTopProductosAsync(desde, hasta, top, cancellationToken);
        return Ok(result);
    }

    [HttpGet("rotacion-stock")]
    [Authorize(Policy = "PERM_REPORTES_VER")]
    [ProducesResponseType(typeof(ReportTableDto<RotacionStockItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReportTableDto<RotacionStockItemDto>>> RotacionStock(
        [FromQuery] DateTimeOffset? desde,
        [FromQuery] DateTimeOffset? hasta,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetRotacionStockAsync(desde, hasta, cancellationToken);
        return Ok(result);
    }

    [HttpGet("stock-inmovilizado")]
    [Authorize(Policy = "PERM_REPORTES_VER")]
    [ProducesResponseType(typeof(ReportTableDto<StockInmovilizadoItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReportTableDto<StockInmovilizadoItemDto>>> StockInmovilizado(
        [FromQuery] DateTimeOffset? desde,
        [FromQuery] DateTimeOffset? hasta,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetStockInmovilizadoAsync(desde, hasta, cancellationToken);
        return Ok(result);
    }
}
