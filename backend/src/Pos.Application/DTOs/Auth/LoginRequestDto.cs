namespace Pos.Application.DTOs.Auth;

public sealed record LoginRequestDto(string Username, string Password, Guid? TenantId, Guid? SucursalId);
