using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class RolePermission : EntityBase
{
    private RolePermission()
    {
    }

    public RolePermission(Guid id, Guid tenantId, Guid roleId, Guid permissionId, DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (roleId == Guid.Empty) throw new ArgumentException("RoleId is required.", nameof(roleId));
        if (permissionId == Guid.Empty) throw new ArgumentException("PermissionId is required.", nameof(permissionId));

        RoleId = roleId;
        PermissionId = permissionId;
    }

    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
}
