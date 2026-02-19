namespace Pos.Application.DTOs.Comprobantes;

public sealed record ComprobanteDto(
    Guid Id,
    Guid VentaId,
    string Estado,
    decimal Total,
    string? Numero,
    string? Provider,
    DateTimeOffset? EmitidoAt,
    DateTimeOffset CreatedAt);

public sealed record FiscalEmitRequestDto(
    Guid ComprobanteId,
    Guid VentaId,
    decimal Total,
    DateTimeOffset Fecha);

public sealed record FiscalEmitResultDto(
    string Provider,
    string Numero,
    string? Payload,
    DateTimeOffset EmitidoAt);
