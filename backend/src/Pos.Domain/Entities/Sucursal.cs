using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class Sucursal : EntityBase
{
    private Sucursal()
    {
    }

    public Sucursal(Guid id, Guid tenantId, string name, string? code, DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));

        Name = name;
        Code = code;
    }

    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
}
