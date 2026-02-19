using Pos.Application.DTOs.Stock;

namespace Pos.Application.DTOs.Recepciones;

public sealed record RecepcionItemDto(
    Guid Id,
    Guid ProductoId,
    string Producto,
    string Sku,
    string Codigo,
    string Descripcion,
    decimal Cantidad,
    decimal? CostoUnitario);

public sealed record RecepcionDto(
    Guid Id,
    Guid PreRecepcionId,
    DateTimeOffset CreatedAt,
    IReadOnlyList<RecepcionItemDto> Items);

public sealed record RecepcionConfirmResultDto(
    RecepcionDto Recepcion,
    IReadOnlyCollection<StockSaldoChangeDto> StockCambios);
