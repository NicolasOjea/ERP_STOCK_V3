namespace Pos.Application.DTOs.Stock;

public sealed record StockSaldoDto(
    Guid ProductoId,
    string Nombre,
    string Sku,
    string Codigo,
    decimal CantidadActual,
    Guid? ProveedorId,
    string? Proveedor);
