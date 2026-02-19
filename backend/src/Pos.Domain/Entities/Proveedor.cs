using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class Proveedor : EntityBase
{
    private Proveedor()
    {
    }

    public Proveedor(
        Guid id,
        Guid tenantId,
        string name,
        string telefono,
        string? cuit,
        string? direccion,
        DateTimeOffset createdAtUtc,
        bool isActive = true)
        : base(id, tenantId, createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(telefono)) throw new ArgumentException("Telefono is required.", nameof(telefono));

        Name = name;
        Telefono = telefono;
        Cuit = cuit;
        Direccion = direccion;
        IsActive = isActive;
    }

    public string Name { get; private set; } = string.Empty;
    public string Telefono { get; private set; } = string.Empty;
    public string? Cuit { get; private set; }
    public string? Direccion { get; private set; }
    public bool IsActive { get; private set; }

    public void Update(
        string name,
        string telefono,
        string? cuit,
        string? direccion,
        bool isActive,
        DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(telefono)) throw new ArgumentException("Telefono is required.", nameof(telefono));

        Name = name;
        Telefono = telefono;
        Cuit = cuit;
        Direccion = direccion;
        IsActive = isActive;
        MarkUpdated(updatedAtUtc);
    }
}
