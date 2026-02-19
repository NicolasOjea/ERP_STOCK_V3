namespace Pos.Application.DTOs.Stock;

public sealed record StockConfigDto(
    Guid ProductoId,
    Guid SucursalId,
    decimal StockMinimo,
    decimal StockDeseado,
    decimal ToleranciaPct);
