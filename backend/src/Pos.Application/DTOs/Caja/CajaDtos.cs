namespace Pos.Application.DTOs.Caja;

public sealed record CajaDto(
    Guid Id,
    string Nombre,
    string? Numero,
    bool IsActive);

public sealed record CajaCreateDto(
    string Numero,
    string Nombre,
    bool? IsActive);
