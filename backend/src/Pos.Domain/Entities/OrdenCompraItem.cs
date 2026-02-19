using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class OrdenCompraItem : EntityBase
{
    private OrdenCompraItem()
    {
    }

    public OrdenCompraItem(
        Guid id,
        Guid tenantId,
        Guid ordenCompraId,
        Guid productoId,
        decimal cantidad,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (ordenCompraId == Guid.Empty) throw new ArgumentException("OrdenCompraId is required.", nameof(ordenCompraId));
        if (productoId == Guid.Empty) throw new ArgumentException("ProductoId is required.", nameof(productoId));
        if (cantidad <= 0) throw new ArgumentException("Cantidad must be greater than 0.", nameof(cantidad));

        OrdenCompraId = ordenCompraId;
        ProductoId = productoId;
        Cantidad = cantidad;
    }

    public Guid OrdenCompraId { get; private set; }
    public Guid ProductoId { get; private set; }
    public decimal Cantidad { get; private set; }
}
