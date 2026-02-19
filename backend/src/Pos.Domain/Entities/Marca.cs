using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class Marca : EntityBase
{
    private Marca()
    {
    }

    public Marca(Guid id, Guid tenantId, string name, DateTimeOffset createdAtUtc, bool isActive = true)
        : base(id, tenantId, createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));

        Name = name;
        IsActive = isActive;
    }

    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    public void Update(string name, bool isActive, DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));

        Name = name;
        IsActive = isActive;
        MarkUpdated(updatedAtUtc);
    }
}
