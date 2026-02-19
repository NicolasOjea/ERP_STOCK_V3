using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class ListaPrecioItem : EntityBase
{
    private ListaPrecioItem()
    {
    }

    public ListaPrecioItem(
        Guid id,
        Guid tenantId,
        Guid listaPrecioId,
        Guid productoId,
        decimal precio,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (listaPrecioId == Guid.Empty) throw new ArgumentException("ListaPrecioId is required.", nameof(listaPrecioId));
        if (productoId == Guid.Empty) throw new ArgumentException("ProductoId is required.", nameof(productoId));
        if (precio <= 0) throw new ArgumentException("Precio must be greater than 0.", nameof(precio));

        ListaPrecioId = listaPrecioId;
        ProductoId = productoId;
        Precio = precio;
    }

    public Guid ListaPrecioId { get; private set; }
    public Guid ProductoId { get; private set; }
    public decimal Precio { get; private set; }

    public void UpdatePrecio(decimal precio, DateTimeOffset updatedAtUtc)
    {
        if (precio <= 0) throw new ArgumentException("Precio must be greater than 0.", nameof(precio));

        Precio = precio;
        MarkUpdated(updatedAtUtc);
    }
}
