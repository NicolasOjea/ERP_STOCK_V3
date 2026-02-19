namespace Pos.Application.DTOs.Caja;

public sealed record CajaResumenDto(
    Guid CajaSesionId,
    Guid CajaId,
    decimal MontoInicial,
    decimal TotalIngresos,
    decimal TotalEgresos,
    decimal SaldoActual,
    int TotalMovimientos,
    IReadOnlyCollection<CajaResumenMedioDto> Medios);

public sealed record CajaResumenMedioDto(
    string Medio,
    decimal Teorico);
