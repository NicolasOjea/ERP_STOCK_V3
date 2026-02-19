namespace Pos.Application.Abstractions;

public interface IUserRoleRepository
{
    Task<bool> UserExistsAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetUserRoleNamesAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<string, Guid>> GetRoleIdsByNamesAsync(
        Guid tenantId,
        IReadOnlyCollection<string> roleNames,
        CancellationToken cancellationToken = default);

    Task ReplaceUserRolesAsync(
        Guid tenantId,
        Guid userId,
        IReadOnlyCollection<Guid> roleIds,
        CancellationToken cancellationToken = default);
}
