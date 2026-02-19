namespace Pos.Application.DTOs.Products;

public sealed record ProductListItemDto(
    Guid Id,
    string Name,
    string Sku,
    string Codigo,
    Guid? CategoriaId,
    string? Categoria,
    Guid? MarcaId,
    string? Marca,
    Guid? ProveedorId,
    string? Proveedor,
    decimal PrecioBase,
    decimal PrecioVenta,
    string PricingMode,
    decimal? MargenGananciaPct,
    bool IsActive);
