using Pos.Application.DTOs.Caja;
using Pos.Application.DTOs.Stock;

namespace Pos.Application.DTOs.Ventas;

public sealed record VentaAnularRequestDto(string Motivo);

public sealed record VentaAnularResultDto(
    VentaDto Venta,
    IReadOnlyCollection<StockSaldoChangeDto> StockCambios,
    IReadOnlyCollection<CajaMovimientoResultDto> CajaMovimientos);
