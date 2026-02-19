using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class Role : EntityBase
{
    private Role()
    {
    }

    public Role(Guid id, Guid tenantId, string name, DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));

        Name = name;
    }

    public string Name { get; private set; } = string.Empty;
}
