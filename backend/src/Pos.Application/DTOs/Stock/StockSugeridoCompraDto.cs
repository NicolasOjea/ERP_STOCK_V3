namespace Pos.Application.DTOs.Stock;

public sealed record StockSugeridoCompraDto(
    decimal TotalSugerido,
    int TotalItems,
    IReadOnlyCollection<StockSugeridoProveedorDto> Proveedores);

public sealed record StockSugeridoProveedorDto(
    Guid? ProveedorId,
    string Proveedor,
    decimal TotalSugerido,
    int TotalItems,
    IReadOnlyCollection<StockSugeridoItemDto> Items);

public sealed record StockSugeridoItemDto(
    Guid ProductoId,
    string Nombre,
    string Sku,
    string CodigoPrincipal,
    decimal StockActual,
    decimal StockMinimo,
    decimal Sugerido);
