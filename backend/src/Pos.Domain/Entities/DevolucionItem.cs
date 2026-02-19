using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class DevolucionItem : EntityBase
{
    private DevolucionItem()
    {
    }

    public DevolucionItem(
        Guid id,
        Guid tenantId,
        Guid devolucionId,
        Guid productoId,
        decimal cantidad,
        decimal precioUnitario,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (devolucionId == Guid.Empty) throw new ArgumentException("DevolucionId is required.", nameof(devolucionId));
        if (productoId == Guid.Empty) throw new ArgumentException("ProductoId is required.", nameof(productoId));
        if (cantidad <= 0) throw new ArgumentException("Cantidad must be > 0.", nameof(cantidad));
        if (precioUnitario < 0) throw new ArgumentException("PrecioUnitario must be >= 0.", nameof(precioUnitario));

        DevolucionId = devolucionId;
        ProductoId = productoId;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
    }

    public Guid DevolucionId { get; private set; }
    public Guid ProductoId { get; private set; }
    public decimal Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
}
