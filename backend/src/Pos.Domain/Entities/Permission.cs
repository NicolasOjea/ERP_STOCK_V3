using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class Permission : EntityBase
{
    private Permission()
    {
    }

    public Permission(Guid id, Guid tenantId, string code, string description, DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code is required.", nameof(code));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));

        Code = code;
        Description = description;
    }

    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
}
