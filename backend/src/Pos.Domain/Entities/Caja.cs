using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class Caja : EntityBase
{
    private Caja()
    {
    }

    public Caja(
        Guid id,
        Guid tenantId,
        Guid sucursalId,
        string name,
        string? numero,
        DateTimeOffset createdAtUtc,
        bool isActive = true)
        : base(id, tenantId, createdAtUtc)
    {
        if (sucursalId == Guid.Empty) throw new ArgumentException("SucursalId is required.", nameof(sucursalId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (numero is not null && string.IsNullOrWhiteSpace(numero)) throw new ArgumentException("Numero is invalid.", nameof(numero));

        SucursalId = sucursalId;
        Name = name;
        Numero = numero;
        IsActive = isActive;
    }

    public Guid SucursalId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Numero { get; private set; }
    public bool IsActive { get; private set; }

    public void Update(string name, bool isActive, DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));

        Name = name;
        IsActive = isActive;
        MarkUpdated(updatedAtUtc);
    }
}
