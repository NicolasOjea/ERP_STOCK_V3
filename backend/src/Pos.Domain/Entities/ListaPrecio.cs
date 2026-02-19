using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class ListaPrecio : EntityBase
{
    private ListaPrecio()
    {
    }

    public ListaPrecio(Guid id, Guid tenantId, string nombre, DateTimeOffset createdAtUtc, bool isActive = true)
        : base(id, tenantId, createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre is required.", nameof(nombre));

        Nombre = nombre;
        IsActive = isActive;
    }

    public string Nombre { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    public void Update(string nombre, bool isActive, DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre is required.", nameof(nombre));

        Nombre = nombre;
        IsActive = isActive;
        MarkUpdated(updatedAtUtc);
    }
}
