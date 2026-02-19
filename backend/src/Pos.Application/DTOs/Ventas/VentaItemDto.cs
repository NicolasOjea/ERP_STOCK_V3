namespace Pos.Application.DTOs.Ventas;

public sealed record VentaItemDto(
    Guid Id,
    Guid ProductoId,
    string Nombre,
    string Sku,
    string Codigo,
    decimal Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal);
