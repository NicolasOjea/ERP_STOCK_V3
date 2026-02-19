namespace Pos.Application.DTOs.Products;

public sealed record ProductCreateDto(
    string Name,
    string Sku,
    Guid? CategoriaId,
    Guid? MarcaId,
    Guid? ProveedorId,
    bool? IsActive,
    decimal? PrecioBase = null,
    decimal? PrecioVenta = null,
    string? PricingMode = null,
    decimal? MargenGananciaPct = null);
