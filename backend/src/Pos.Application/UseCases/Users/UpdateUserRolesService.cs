using System.Text.Json;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Users;
using Pos.Domain.Enums;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.Users;

public sealed class UpdateUserRolesService
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRequestContext _requestContext;
    private readonly IAuditLogService _auditLogService;

    public UpdateUserRolesService(
        IUserRoleRepository userRoleRepository,
        IRequestContext requestContext,
        IAuditLogService auditLogService)
    {
        _userRoleRepository = userRoleRepository;
        _requestContext = requestContext;
        _auditLogService = auditLogService;
    }

    public async Task UpdateRolesAsync(Guid userId, UpdateUserRolesRequestDto request, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["userId"] = new[] { "El usuario es obligatorio." }
                });
        }

        if (request is null || request.Roles is null || request.Roles.Count == 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["roles"] = new[] { "Debe especificar al menos un rol." }
                });
        }

        var tenantId = _requestContext.TenantId;
        if (tenantId == Guid.Empty)
        {
            throw new UnauthorizedException("Contexto de tenant invalido.");
        }

        var normalizedRoles = request.Roles
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .Select(r => r.Trim().ToUpperInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedRoles.Count == 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["roles"] = new[] { "Debe especificar al menos un rol valido." }
                });
        }

        var userExists = await _userRoleRepository.UserExistsAsync(tenantId, userId, cancellationToken);
        if (!userExists)
        {
            throw new NotFoundException("Usuario no encontrado.");
        }

        var roleIdMap = await _userRoleRepository.GetRoleIdsByNamesAsync(tenantId, normalizedRoles, cancellationToken);
        var missingRoles = normalizedRoles.Where(role => !roleIdMap.ContainsKey(role)).ToList();
        if (missingRoles.Count > 0)
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["roles"] = new[] { $"Roles invalidos: {string.Join(", ", missingRoles)}" }
                });
        }

        var beforeRoles = await _userRoleRepository.GetUserRoleNamesAsync(tenantId, userId, cancellationToken);

        await _userRoleRepository.ReplaceUserRolesAsync(tenantId, userId, roleIdMap.Values.ToList(), cancellationToken);

        var afterRoles = normalizedRoles.OrderBy(r => r, StringComparer.OrdinalIgnoreCase).ToList();
        var beforeJson = JsonSerializer.Serialize(new { roles = beforeRoles.OrderBy(r => r, StringComparer.OrdinalIgnoreCase) });
        var afterJson = JsonSerializer.Serialize(new { roles = afterRoles });

        await _auditLogService.LogAsync(
            "UserRole",
            userId.ToString(),
            AuditAction.RoleChange,
            beforeJson,
            afterJson,
            null,
            cancellationToken);
    }
}
