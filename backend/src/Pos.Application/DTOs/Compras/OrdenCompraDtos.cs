namespace Pos.Application.DTOs.Compras;

public sealed record OrdenCompraItemDto(
    Guid Id,
    Guid ProductoId,
    string Producto,
    string Sku,
    decimal Cantidad);

public sealed record OrdenCompraDto(
    Guid Id,
    Guid SucursalId,
    Guid? ProveedorId,
    string? Proveedor,
    string Estado,
    DateTimeOffset CreatedAt,
    IReadOnlyList<OrdenCompraItemDto> Items);

public sealed record OrdenCompraListItemDto(
    Guid Id,
    Guid? ProveedorId,
    string? Proveedor,
    string Estado,
    DateTimeOffset CreatedAt,
    int TotalItems);

public sealed record OrdenCompraCreateDto(
    Guid? ProveedorId,
    IReadOnlyList<OrdenCompraItemCreateDto> Items);

public sealed record OrdenCompraItemCreateDto(
    Guid ProductoId,
    decimal Cantidad);
