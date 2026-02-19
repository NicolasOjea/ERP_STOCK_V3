using Pos.Domain.Common;
using Pos.Domain.Enums;

namespace Pos.Domain.Entities;

public sealed class OrdenCompra : EntityBase
{
    private OrdenCompra()
    {
    }

    public OrdenCompra(
        Guid id,
        Guid tenantId,
        Guid sucursalId,
        Guid? proveedorId,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (sucursalId == Guid.Empty) throw new ArgumentException("SucursalId is required.", nameof(sucursalId));

        SucursalId = sucursalId;
        ProveedorId = proveedorId;
        Estado = OrdenCompraEstado.Borrador;
    }

    public Guid SucursalId { get; private set; }
    public Guid? ProveedorId { get; private set; }
    public OrdenCompraEstado Estado { get; private set; }

    public void CambiarEstado(OrdenCompraEstado estado, DateTimeOffset updatedAtUtc)
    {
        Estado = estado;
        MarkUpdated(updatedAtUtc);
    }
}
