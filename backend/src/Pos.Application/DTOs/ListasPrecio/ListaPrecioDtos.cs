namespace Pos.Application.DTOs.ListasPrecio;

public sealed record ListaPrecioDto(
    Guid Id,
    string Nombre,
    bool IsActive);

public sealed record ListaPrecioCreateDto(
    string Nombre,
    bool? IsActive);

public sealed record ListaPrecioUpdateDto(
    string? Nombre,
    bool? IsActive);

public sealed record ListaPrecioItemUpsertDto(
    Guid ProductoId,
    decimal Precio);

public sealed record ListaPrecioItemsUpdateDto(
    IReadOnlyList<ListaPrecioItemUpsertDto> Items);
