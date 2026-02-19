namespace Pos.Application.DTOs.Caja;

public sealed record CajaCierreRequestDto(
    decimal EfectivoContado,
    IReadOnlyCollection<CajaCierreMedioDto> Medios,
    string? MotivoDiferencia);

public sealed record CajaCierreMedioDto(string Medio, decimal Contado);

public sealed record CajaCierreMedioResultDto(
    string Medio,
    decimal Teorico,
    decimal Contado,
    decimal Diferencia);

public sealed record CajaCierreResultDto(
    Guid SesionId,
    Guid CajaId,
    Guid SucursalId,
    string Estado,
    DateTimeOffset CierreAt,
    decimal TotalTeorico,
    decimal TotalContado,
    decimal Diferencia,
    IReadOnlyCollection<CajaCierreMedioResultDto> Medios);
