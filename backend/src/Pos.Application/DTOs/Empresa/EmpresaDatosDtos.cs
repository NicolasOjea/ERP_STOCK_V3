namespace Pos.Application.DTOs.Empresa;

public sealed record EmpresaDatosDto(
    Guid Id,
    string RazonSocial,
    string? Cuit,
    string? Telefono,
    string? Direccion,
    string? Email,
    string? Web,
    string? Observaciones);

public sealed record EmpresaDatosUpsertDto(
    string RazonSocial,
    string? Cuit,
    string? Telefono,
    string? Direccion,
    string? Email,
    string? Web,
    string? Observaciones);
