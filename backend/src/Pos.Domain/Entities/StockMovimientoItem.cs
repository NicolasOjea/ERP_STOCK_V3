using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class StockMovimientoItem : EntityBase
{
    private StockMovimientoItem()
    {
    }

    public StockMovimientoItem(
        Guid id,
        Guid tenantId,
        Guid movimientoId,
        Guid productoId,
        decimal cantidad,
        bool esIngreso,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (movimientoId == Guid.Empty) throw new ArgumentException("MovimientoId is required.", nameof(movimientoId));
        if (productoId == Guid.Empty) throw new ArgumentException("ProductoId is required.", nameof(productoId));
        if (cantidad <= 0) throw new ArgumentException("Cantidad must be greater than 0.", nameof(cantidad));

        MovimientoId = movimientoId;
        ProductoId = productoId;
        Cantidad = cantidad;
        EsIngreso = esIngreso;
    }

    public Guid MovimientoId { get; private set; }
    public Guid ProductoId { get; private set; }
    public decimal Cantidad { get; private set; }
    public bool EsIngreso { get; private set; }
}
