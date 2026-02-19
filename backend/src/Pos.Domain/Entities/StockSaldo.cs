using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class StockSaldo : EntityBase
{
    private StockSaldo()
    {
    }

    public StockSaldo(
        Guid id,
        Guid tenantId,
        Guid productoId,
        Guid sucursalId,
        decimal cantidadActual,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (productoId == Guid.Empty) throw new ArgumentException("ProductoId is required.", nameof(productoId));
        if (sucursalId == Guid.Empty) throw new ArgumentException("SucursalId is required.", nameof(sucursalId));

        ProductoId = productoId;
        SucursalId = sucursalId;
        CantidadActual = cantidadActual;
    }

    public Guid ProductoId { get; private set; }
    public Guid SucursalId { get; private set; }
    public decimal CantidadActual { get; private set; }

    public void SetCantidad(decimal cantidadActual, DateTimeOffset updatedAtUtc)
    {
        CantidadActual = cantidadActual;
        MarkUpdated(updatedAtUtc);
    }
}
