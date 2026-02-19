using Microsoft.EntityFrameworkCore;
using Pos.Application.Abstractions;
using Pos.Domain.Entities;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class UserRoleRepository : IUserRoleRepository
{
    private readonly PosDbContext _dbContext;

    public UserRoleRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> UserExistsAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Usuarios.AsNoTracking()
            .AnyAsync(u => u.TenantId == tenantId && u.Id == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetUserRoleNamesAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await (from ur in _dbContext.UsuarioRoles.AsNoTracking()
            join role in _dbContext.Roles.AsNoTracking() on ur.RoleId equals role.Id
            where ur.TenantId == tenantId && ur.UserId == userId && role.TenantId == tenantId
            select role.Name)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<string, Guid>> GetRoleIdsByNamesAsync(
        Guid tenantId,
        IReadOnlyCollection<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        var names = roleNames
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r.Trim().ToUpperInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (names.Count == 0)
        {
            return new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        }

        var roles = await _dbContext.Roles.AsNoTracking()
            .Where(r => r.TenantId == tenantId && names.Contains(r.Name))
            .ToListAsync(cancellationToken);

        return roles.ToDictionary(r => r.Name, r => r.Id, StringComparer.OrdinalIgnoreCase);
    }

    public async Task ReplaceUserRolesAsync(
        Guid tenantId,
        Guid userId,
        IReadOnlyCollection<Guid> roleIds,
        CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.UsuarioRoles
            .Where(ur => ur.TenantId == tenantId && ur.UserId == userId)
            .ToListAsync(cancellationToken);

        var desiredRoleIds = roleIds.Distinct().ToHashSet();

        var toRemove = existing.Where(ur => !desiredRoleIds.Contains(ur.RoleId)).ToList();
        if (toRemove.Count > 0)
        {
            _dbContext.UsuarioRoles.RemoveRange(toRemove);
        }

        var existingRoleIds = existing.Select(ur => ur.RoleId).ToHashSet();
        var toAdd = desiredRoleIds.Where(roleId => !existingRoleIds.Contains(roleId)).ToList();
        if (toAdd.Count > 0)
        {
            var now = DateTimeOffset.UtcNow;
            foreach (var roleId in toAdd)
            {
                _dbContext.UsuarioRoles.Add(new UserRole(Guid.NewGuid(), tenantId, userId, roleId, now));
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
