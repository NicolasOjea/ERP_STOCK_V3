using Pos.Application.DTOs.Caja;
using Pos.Application.DTOs.Stock;

namespace Pos.Application.DTOs.Devoluciones;

public sealed record DevolucionItemCreateDto(Guid ProductoId, decimal Cantidad);

public sealed record DevolucionCreateDto(Guid VentaId, string Motivo, IReadOnlyCollection<DevolucionItemCreateDto> Items);

public sealed record DevolucionItemDto(
    Guid Id,
    Guid ProductoId,
    string Producto,
    string Sku,
    decimal Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal);

public sealed record DevolucionDto(
    Guid Id,
    Guid VentaId,
    string Motivo,
    decimal Total,
    DateTimeOffset CreatedAt,
    IReadOnlyList<DevolucionItemDto> Items);

public sealed record NotaCreditoInternaDto(
    Guid Id,
    Guid DevolucionId,
    decimal Total,
    string Motivo,
    DateTimeOffset CreatedAt);

public sealed record DevolucionResultDto(
    DevolucionDto Devolucion,
    NotaCreditoInternaDto NotaCredito,
    IReadOnlyCollection<StockSaldoChangeDto> StockCambios,
    IReadOnlyCollection<CajaMovimientoResultDto> CajaMovimientos);
