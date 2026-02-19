namespace Pos.Application.DTOs.PreRecepciones;

public sealed record PreRecepcionItemDto(
    Guid Id,
    string Codigo,
    string Descripcion,
    decimal Cantidad,
    decimal? CostoUnitario,
    string Estado,
    Guid? ProductoId,
    string? Producto,
    string? ProductoSku);

public sealed record PreRecepcionDto(
    Guid Id,
    Guid DocumentoCompraId,
    DateTimeOffset CreatedAt,
    IReadOnlyList<PreRecepcionItemDto> Items);

public sealed record PreRecepcionCreateDto(
    Guid DocumentoCompraId);

public sealed record PreRecepcionUpdateDto(
    IReadOnlyList<PreRecepcionItemUpdateDto> Items);

public sealed record PreRecepcionItemUpdateDto(
    Guid ItemId,
    Guid? ProductoId,
    decimal? Cantidad);
