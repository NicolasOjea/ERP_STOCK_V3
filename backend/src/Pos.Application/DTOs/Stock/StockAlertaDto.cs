namespace Pos.Application.DTOs.Stock;

public sealed record StockAlertaDto(
    Guid ProductoId,
    string Nombre,
    string Sku,
    Guid? ProveedorId,
    string? Proveedor,
    decimal CantidadActual,
    decimal StockMinimo,
    decimal StockDeseado,
    decimal ToleranciaPct,
    decimal Sugerido,
    string Nivel);
