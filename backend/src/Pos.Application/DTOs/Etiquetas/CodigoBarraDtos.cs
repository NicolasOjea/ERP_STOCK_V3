namespace Pos.Application.DTOs.Etiquetas;

public sealed record CodigosBarraRequestDto(IReadOnlyCollection<Guid> ProductoIds);

public sealed record CodigoBarraProductoDto(
    Guid ProductoId,
    string Nombre,
    string Sku,
    Guid? ProveedorId,
    string? ProveedorNombre);

public sealed record CodigoBarraItemPdfDto(
    string Nombre,
    string Sku);

public sealed record CodigoBarraProveedorPdfDto(
    string Proveedor,
    IReadOnlyList<CodigoBarraItemPdfDto> Items);

public sealed record CodigoBarraPdfDataDto(
    IReadOnlyList<CodigoBarraProveedorPdfDto> Proveedores);
