using Pos.Domain.Common;
using Pos.Domain.Enums;

namespace Pos.Domain.Entities;

public sealed class PreRecepcionItem : EntityBase
{
    private PreRecepcionItem()
    {
    }

    public PreRecepcionItem(
        Guid id,
        Guid tenantId,
        Guid preRecepcionId,
        Guid documentoCompraItemId,
        string codigo,
        string descripcion,
        decimal cantidad,
        decimal? costoUnitario,
        Guid? productoId,
        PreRecepcionItemEstado estado,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (preRecepcionId == Guid.Empty) throw new ArgumentException("PreRecepcionId is required.", nameof(preRecepcionId));
        if (documentoCompraItemId == Guid.Empty) throw new ArgumentException("DocumentoCompraItemId is required.", nameof(documentoCompraItemId));
        if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("Codigo is required.", nameof(codigo));
        if (string.IsNullOrWhiteSpace(descripcion)) throw new ArgumentException("Descripcion is required.", nameof(descripcion));
        if (cantidad <= 0) throw new ArgumentException("Cantidad must be greater than 0.", nameof(cantidad));

        PreRecepcionId = preRecepcionId;
        DocumentoCompraItemId = documentoCompraItemId;
        Codigo = codigo;
        Descripcion = descripcion;
        Cantidad = cantidad;
        CostoUnitario = costoUnitario;
        ProductoId = productoId;
        Estado = estado;
    }

    public Guid PreRecepcionId { get; private set; }
    public Guid DocumentoCompraItemId { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public decimal Cantidad { get; private set; }
    public decimal? CostoUnitario { get; private set; }
    public Guid? ProductoId { get; private set; }
    public PreRecepcionItemEstado Estado { get; private set; }

    public void ActualizarCantidad(decimal cantidad, DateTimeOffset updatedAtUtc)
    {
        if (cantidad <= 0) throw new ArgumentException("Cantidad must be greater than 0.", nameof(cantidad));

        Cantidad = cantidad;
        MarkUpdated(updatedAtUtc);
    }

    public void AsignarProducto(Guid? productoId, PreRecepcionItemEstado estado, DateTimeOffset updatedAtUtc)
    {
        ProductoId = productoId;
        Estado = estado;
        MarkUpdated(updatedAtUtc);
    }
}
