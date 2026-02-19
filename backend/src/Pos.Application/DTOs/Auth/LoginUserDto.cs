namespace Pos.Application.DTOs.Auth;

public sealed record LoginUserDto(
    Guid TenantId,
    Guid SucursalId,
    Guid UserId,
    string PasswordHash,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions,
    bool IsActive);
