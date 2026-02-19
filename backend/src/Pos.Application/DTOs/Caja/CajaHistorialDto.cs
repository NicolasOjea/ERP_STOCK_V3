namespace Pos.Application.DTOs.Caja;

public sealed record CajaHistorialDto(
    Guid SesionId,
    Guid CajaId,
    string Cajero,
    string Turno,
    DateTimeOffset AperturaAt,
    DateTimeOffset? CierreAt,
    decimal MontoInicial,
    decimal TotalEfectivo,
    decimal TotalTarjeta,
    decimal TotalTransferencia,
    decimal TotalOtro,
    decimal TotalAplicativo,
    decimal TotalContado,
    decimal Diferencia,
    string? MotivoDiferencia,
    long? VentaDesde,
    long? VentaHasta);
