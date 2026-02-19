namespace Pos.Application.DTOs.Stock;

public sealed record StockMovimientoCreateDto(
    string Tipo,
    string Motivo,
    IReadOnlyCollection<StockMovimientoItemCreateDto> Items);

public sealed record StockMovimientoItemCreateDto(
    Guid ProductoId,
    decimal Cantidad,
    bool EsIngreso);
