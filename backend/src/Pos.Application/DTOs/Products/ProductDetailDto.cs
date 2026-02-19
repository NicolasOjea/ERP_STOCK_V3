namespace Pos.Application.DTOs.Products;

public sealed record ProductDetailDto(
    Guid Id,
    string Name,
    string Sku,
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
    bool IsActive,
    IReadOnlyCollection<ProductCodeDto> Codes);
