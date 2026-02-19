namespace Pos.Application.DTOs.Users;

public sealed record UpdateUserRolesRequestDto(IReadOnlyCollection<string> Roles);
