using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class Tenant : EntityBase
{
    private Tenant()
    {
    }

    public Tenant(Guid id, string name, DateTimeOffset createdAtUtc, bool isActive = true)
        : base(id, id, createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));

        Name = name;
        IsActive = isActive;
    }

    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
}
