using Pos.Application.DTOs.Auth;

namespace Pos.Application.Abstractions;

public interface IAuthRepository
{
    Task<LoginUserDto?> GetLoginUserAsync(
        string username,
        Guid? tenantId,
        Guid? sucursalId,
        CancellationToken cancellationToken = default);
}
