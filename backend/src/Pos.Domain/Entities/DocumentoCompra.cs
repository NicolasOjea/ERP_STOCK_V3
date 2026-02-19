using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class DocumentoCompra : EntityBase
{
    private DocumentoCompra()
    {
    }

    public DocumentoCompra(
        Guid id,
        Guid tenantId,
        Guid sucursalId,
        Guid? proveedorId,
        string numero,
        DateTime fecha,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (sucursalId == Guid.Empty) throw new ArgumentException("SucursalId is required.", nameof(sucursalId));
        if (string.IsNullOrWhiteSpace(numero)) throw new ArgumentException("Numero is required.", nameof(numero));

        SucursalId = sucursalId;
        ProveedorId = proveedorId;
        Numero = numero;
        Fecha = fecha.Date;
    }

    public Guid SucursalId { get; private set; }
    public Guid? ProveedorId { get; private set; }
    public string Numero { get; private set; } = string.Empty;
    public DateTime Fecha { get; private set; }
}
