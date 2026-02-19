namespace Pos.Application.DTOs.Auth;

public sealed record JwtTokenRequest(
    Guid TenantId,
    Guid SucursalId,
    Guid UserId,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);
