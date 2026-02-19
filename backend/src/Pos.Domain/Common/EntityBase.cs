namespace Pos.Domain.Common;

public abstract class EntityBase
{
    protected EntityBase()
    {
        Id = Guid.Empty;
        TenantId = Guid.Empty;
        CreatedAt = DateTimeOffset.MinValue;
        UpdatedAt = DateTimeOffset.MinValue;
    }

    protected EntityBase(Guid id, Guid tenantId, DateTimeOffset createdAtUtc)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(id));
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty.", nameof(tenantId));

        Id = id;
        TenantId = tenantId;
        CreatedAt = createdAtUtc;
        UpdatedAt = createdAtUtc;
    }

    public Guid Id { get; protected set; }
    public Guid TenantId { get; protected set; }
    public DateTimeOffset CreatedAt { get; protected set; }
    public DateTimeOffset UpdatedAt { get; protected set; }
    public DateTimeOffset? DeletedAt { get; protected set; }

    public void MarkUpdated(DateTimeOffset updatedAtUtc)
    {
        UpdatedAt = updatedAtUtc;
    }

    public void SoftDelete(DateTimeOffset deletedAtUtc)
    {
        DeletedAt = deletedAtUtc;
        UpdatedAt = deletedAtUtc;
    }
}
