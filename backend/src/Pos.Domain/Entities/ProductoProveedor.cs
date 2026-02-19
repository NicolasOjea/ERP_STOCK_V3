using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class ProductoProveedor : EntityBase
{
    private ProductoProveedor()
    {
    }

    public ProductoProveedor(
        Guid id,
        Guid tenantId,
        Guid productoId,
        Guid proveedorId,
        bool esPrincipal,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (productoId == Guid.Empty) throw new ArgumentException("ProductoId is required.", nameof(productoId));
        if (proveedorId == Guid.Empty) throw new ArgumentException("ProveedorId is required.", nameof(proveedorId));

        ProductoId = productoId;
        ProveedorId = proveedorId;
        EsPrincipal = esPrincipal;
    }

    public Guid ProductoId { get; private set; }
    public Guid ProveedorId { get; private set; }
    public bool EsPrincipal { get; private set; }

    public void SetPrincipal(bool esPrincipal, DateTimeOffset updatedAtUtc)
    {
        EsPrincipal = esPrincipal;
        MarkUpdated(updatedAtUtc);
    }
}
