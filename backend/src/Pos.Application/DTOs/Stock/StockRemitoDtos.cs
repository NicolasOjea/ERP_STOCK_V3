namespace Pos.Application.DTOs.Stock;

public sealed record StockRemitoItemDto(Guid ProductoId, decimal Cantidad);

public sealed record StockRemitoRequestDto(
    IReadOnlyCollection<StockRemitoItemDto> Items,
    Guid? ProveedorId = null);

public sealed record StockRemitoProductoDto(
    Guid ProductoId,
    string Nombre,
    string Sku,
    Guid? ProveedorId,
    string? ProveedorNombre,
    string? ProveedorTelefono,
    string? ProveedorCuit,
    string? ProveedorDireccion);

public sealed record StockRemitoHeaderDto(
    string EmpresaNombre,
    string SucursalNombre);

public sealed record StockRemitoPdfItemDto(
    string Nombre,
    string Sku,
    decimal Cantidad);

public sealed record StockRemitoPdfProveedorDto(
    string Nombre,
    string? Telefono,
    string? Cuit,
    string? Direccion,
    IReadOnlyList<StockRemitoPdfItemDto> Items);

public sealed record StockRemitoPdfDataDto(
    DateTimeOffset Fecha,
    string RemitoNumero,
    StockRemitoHeaderDto Header,
    IReadOnlyList<StockRemitoPdfProveedorDto> Proveedores);
