namespace Pos.Application.DTOs.DocumentosCompra;

public sealed record DocumentoCompraItemDto(
    Guid Id,
    string Codigo,
    string Descripcion,
    decimal Cantidad,
    decimal? CostoUnitario);

public sealed record DocumentoCompraDto(
    Guid Id,
    Guid? ProveedorId,
    string Numero,
    DateTime Fecha,
    DateTimeOffset CreatedAt,
    IReadOnlyList<DocumentoCompraItemDto> Items);

public sealed record DocumentoCompraParseResultDto(
    Guid DocumentoCompraId,
    IReadOnlyList<DocumentoCompraItemDto> Items);

public sealed record ParsedDocumentDto(
    Guid? ProveedorId,
    string Numero,
    DateTime Fecha,
    IReadOnlyList<ParsedDocumentItemDto> Items);

public sealed record ParsedDocumentItemDto(
    string Codigo,
    string Descripcion,
    decimal Cantidad,
    decimal? CostoUnitario);
