namespace Pos.Application.DTOs.Products;

public sealed record ProductProveedorDto(
    Guid Id,
    Guid ProveedorId,
    string Proveedor,
    bool EsPrincipal);

public sealed record ProductProveedorCreateDto(
    Guid ProveedorId,
    bool? EsPrincipal);

public sealed record ProductProveedorUpdateDto(
    bool EsPrincipal);
