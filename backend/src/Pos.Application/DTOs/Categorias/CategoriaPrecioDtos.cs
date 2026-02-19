namespace Pos.Application.DTOs.Categorias;

public sealed record CategoriaPrecioDto(
    Guid Id,
    string Name,
    decimal MargenGananciaPct,
    bool IsActive);

public sealed record CategoriaPrecioCreateDto(
    string Name,
    decimal? MargenGananciaPct,
    bool? IsActive);

public sealed record CategoriaPrecioUpdateDto(
    string? Name,
    decimal? MargenGananciaPct,
    bool? IsActive,
    bool? AplicarAProductos);
