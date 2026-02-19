namespace Pos.Application.DTOs.Products;

public sealed record ProductUpdateDto(
    string? Name,
    string? Sku,
    Guid? CategoriaId,
    Guid? MarcaId,
    Guid? ProveedorId,
    bool? IsActive,
    decimal? PrecioBase,
    decimal? PrecioVenta,
    string? PricingMode = null,
    decimal? MargenGananciaPct = null);
