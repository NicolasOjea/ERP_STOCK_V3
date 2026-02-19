using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class DocumentoCompraItem : EntityBase
{
    private DocumentoCompraItem()
    {
    }

    public DocumentoCompraItem(
        Guid id,
        Guid tenantId,
        Guid documentoCompraId,
        string codigo,
        string descripcion,
        decimal cantidad,
        decimal? costoUnitario,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (documentoCompraId == Guid.Empty) throw new ArgumentException("DocumentoCompraId is required.", nameof(documentoCompraId));
        if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("Codigo is required.", nameof(codigo));
        if (string.IsNullOrWhiteSpace(descripcion)) throw new ArgumentException("Descripcion is required.", nameof(descripcion));
        if (cantidad <= 0) throw new ArgumentException("Cantidad must be greater than 0.", nameof(cantidad));

        DocumentoCompraId = documentoCompraId;
        Codigo = codigo;
        Descripcion = descripcion;
        Cantidad = cantidad;
        CostoUnitario = costoUnitario;
    }

    public Guid DocumentoCompraId { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public decimal Cantidad { get; private set; }
    public decimal? CostoUnitario { get; private set; }
}
