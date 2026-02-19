using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class UserRole : EntityBase
{
    private UserRole()
    {
    }

    public UserRole(Guid id, Guid tenantId, Guid userId, Guid roleId, DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId is required.", nameof(userId));
        if (roleId == Guid.Empty) throw new ArgumentException("RoleId is required.", nameof(roleId));

        UserId = userId;
        RoleId = roleId;
    }

    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
}
