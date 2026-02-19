namespace Pos.Application.DTOs.Caja;

public sealed record CajaSesionDto(
    Guid Id,
    Guid CajaId,
    Guid SucursalId,
    string Turno,
    decimal MontoInicial,
    DateTimeOffset AperturaAt,
    string Estado);
