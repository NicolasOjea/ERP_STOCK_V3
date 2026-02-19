namespace Pos.Application.DTOs.Caja;

public sealed record CajaMovimientoCreateDto(string Tipo, decimal Monto, string Motivo, string? MedioPago);
