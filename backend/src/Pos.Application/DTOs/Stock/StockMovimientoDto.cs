namespace Pos.Application.DTOs.Stock;

public sealed record StockMovimientoDto(
    Guid Id,
    string Tipo,
    string Motivo,
    DateTimeOffset Fecha,
    long? VentaNumero,
    bool? VentaFacturada,
    IReadOnlyCollection<StockMovimientoItemDto> Items);

public sealed record StockMovimientoItemDto(
    Guid Id,
    Guid ProductoId,
    string Nombre,
    string Sku,
    decimal Cantidad,
    bool EsIngreso,
    decimal? SaldoAntes,
    decimal? SaldoDespues);

public sealed record StockSaldoChangeDto(
    Guid MovimientoId,
    Guid MovimientoItemId,
    Guid ProductoId,
    decimal SaldoAntes,
    decimal SaldoDespues);

public sealed record StockMovimientoRegisterResult(
    StockMovimientoDto Movimiento,
    IReadOnlyCollection<StockSaldoChangeDto> Cambios);
