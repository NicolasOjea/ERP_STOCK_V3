using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class VentaItem : EntityBase
{
    private VentaItem()
    {
    }

    public VentaItem(
        Guid id,
        Guid tenantId,
        Guid ventaId,
        Guid productoId,
        string codigo,
        decimal cantidad,
        decimal precioUnitario,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (ventaId == Guid.Empty) throw new ArgumentException("VentaId is required.", nameof(ventaId));
        if (productoId == Guid.Empty) throw new ArgumentException("ProductoId is required.", nameof(productoId));
        if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("Codigo is required.", nameof(codigo));
        if (cantidad <= 0) throw new ArgumentException("Cantidad must be greater than 0.", nameof(cantidad));
        if (precioUnitario < 0) throw new ArgumentException("PrecioUnitario must be >= 0.", nameof(precioUnitario));

        VentaId = ventaId;
        ProductoId = productoId;
        Codigo = codigo;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
    }

    public Guid VentaId { get; private set; }
    public Guid ProductoId { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public decimal Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }

    public void Incrementar(decimal cantidad, DateTimeOffset updatedAtUtc)
    {
        if (cantidad <= 0) throw new ArgumentException("Cantidad must be greater than 0.", nameof(cantidad));

        Cantidad += cantidad;
        MarkUpdated(updatedAtUtc);
    }

    public void ActualizarCantidad(decimal cantidad, DateTimeOffset updatedAtUtc)
    {
        if (cantidad <= 0) throw new ArgumentException("Cantidad must be greater than 0.", nameof(cantidad));

        Cantidad = cantidad;
        MarkUpdated(updatedAtUtc);
    }
}
