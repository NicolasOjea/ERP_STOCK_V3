using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class Categoria : EntityBase
{
    private Categoria()
    {
    }

    public Categoria(
        Guid id,
        Guid tenantId,
        string name,
        DateTimeOffset createdAtUtc,
        decimal margenGananciaPct = 30m,
        bool isActive = true)
        : base(id, tenantId, createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (margenGananciaPct < 0) throw new ArgumentException("MargenGananciaPct must be >= 0.", nameof(margenGananciaPct));

        Name = name;
        MargenGananciaPct = margenGananciaPct;
        IsActive = isActive;
    }

    public string Name { get; private set; } = string.Empty;
    public decimal MargenGananciaPct { get; private set; }
    public bool IsActive { get; private set; }

    public void Update(string name, decimal margenGananciaPct, bool isActive, DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (margenGananciaPct < 0) throw new ArgumentException("MargenGananciaPct must be >= 0.", nameof(margenGananciaPct));

        Name = name;
        MargenGananciaPct = margenGananciaPct;
        IsActive = isActive;
        MarkUpdated(updatedAtUtc);
    }
}
