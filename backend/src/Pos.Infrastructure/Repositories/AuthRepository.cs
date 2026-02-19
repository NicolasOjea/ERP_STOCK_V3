using Microsoft.EntityFrameworkCore;
using Pos.Domain.Enums;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Auth;
using Pos.Infrastructure.Persistence;

namespace Pos.Infrastructure.Repositories;

public sealed class AuthRepository : IAuthRepository
{
    private readonly PosDbContext _dbContext;

    public AuthRepository(PosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LoginUserDto?> GetLoginUserAsync(
        string username,
        Guid? tenantId,
        Guid? sucursalId,
        CancellationToken cancellationToken = default)
    {
        var userQuery = _dbContext.Usuarios.AsNoTracking().Where(u => u.Username == username);
        if (tenantId.HasValue)
        {
            userQuery = userQuery.Where(u => u.TenantId == tenantId.Value);
        }

        var candidates = await userQuery.Take(2).ToListAsync(cancellationToken);
        if (candidates.Count != 1)
        {
            return null;
        }

        var user = candidates[0];

        var resolvedSucursalId = sucursalId;
        if (resolvedSucursalId.HasValue)
        {
            var exists = await _dbContext.Sucursales
                .AsNoTracking()
                .AnyAsync(s => s.Id == resolvedSucursalId.Value && s.TenantId == user.TenantId, cancellationToken);

            if (!exists)
            {
                return null;
            }
        }
        else
        {
            resolvedSucursalId = await _dbContext.Sucursales
                .AsNoTracking()
                .Where(s => s.TenantId == user.TenantId)
                .OrderBy(s => s.Name)
                .Select(s => s.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (resolvedSucursalId == Guid.Empty)
            {
                return null;
            }
        }

        var roles = await (from ur in _dbContext.UsuarioRoles.AsNoTracking()
            join r in _dbContext.Roles.AsNoTracking() on ur.RoleId equals r.Id
            where ur.TenantId == user.TenantId && ur.UserId == user.Id && r.TenantId == user.TenantId
            select r.Name).Distinct().ToListAsync(cancellationToken);

        var permissions = await (from ur in _dbContext.UsuarioRoles.AsNoTracking()
            join rp in _dbContext.RolPermisos.AsNoTracking() on ur.RoleId equals rp.RoleId
            join p in _dbContext.Permisos.AsNoTracking() on rp.PermissionId equals p.Id
            where ur.TenantId == user.TenantId
                  && ur.UserId == user.Id
                  && rp.TenantId == user.TenantId
                  && p.TenantId == user.TenantId
            select p.Code).Distinct().ToListAsync(cancellationToken);

        if (roles.Contains("ADMIN"))
        {
            permissions = PermissionCodes.All.Distinct().ToList();
        }

        return new LoginUserDto(
            user.TenantId,
            resolvedSucursalId.Value,
            user.Id,
            user.PasswordHash,
            roles,
            permissions,
            user.IsActive);
    }
}
