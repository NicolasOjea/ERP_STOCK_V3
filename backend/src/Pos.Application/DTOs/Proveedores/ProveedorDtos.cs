namespace Pos.Application.DTOs.Proveedores;

public sealed record ProveedorDto(
    Guid Id,
    string Name,
    string Telefono,
    string? Cuit,
    string? Direccion,
    bool IsActive);

public sealed record ProveedorCreateDto(
    string Name,
    string Telefono,
    string? Cuit,
    string? Direccion,
    bool? IsActive);

public sealed record ProveedorUpdateDto(
    string? Name,
    string? Telefono,
    string? Cuit,
    string? Direccion,
    bool? IsActive);

public sealed record ProveedorDeleteProductOptionDto(
    Guid Id,
    string Name,
    string Sku,
    bool CanDelete,
    string? BlockReason);

public sealed record ProveedorDeletePreviewDto(
    Guid ProveedorId,
    string ProveedorNombre,
    IReadOnlyList<ProveedorDeleteProductOptionDto> Productos);

public sealed record ProveedorDeleteRequestDto(
    IReadOnlyList<Guid> ProductIdsToDelete);
