namespace Pos.Application.DTOs.Caja;

public sealed record CajaSesionAbrirDto(Guid CajaId, decimal MontoInicial, string Turno);
