using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class RecepcionItem : EntityBase
{
    private RecepcionItem()
    {
    }

    public RecepcionItem(
        Guid id,
        Guid tenantId,
        Guid recepcionId,
        Guid preRecepcionItemId,
        Guid productoId,
        string codigo,
        string descripcion,
        decimal cantidad,
        decimal? costoUnitario,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (recepcionId == Guid.Empty) throw new ArgumentException("RecepcionId is required.", nameof(recepcionId));
        if (preRecepcionItemId == Guid.Empty) throw new ArgumentException("PreRecepcionItemId is required.", nameof(preRecepcionItemId));
        if (productoId == Guid.Empty) throw new ArgumentException("ProductoId is required.", nameof(productoId));
        if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("Codigo is required.", nameof(codigo));
        if (string.IsNullOrWhiteSpace(descripcion)) throw new ArgumentException("Descripcion is required.", nameof(descripcion));
        if (cantidad <= 0) throw new ArgumentException("Cantidad must be greater than 0.", nameof(cantidad));

        RecepcionId = recepcionId;
        PreRecepcionItemId = preRecepcionItemId;
        ProductoId = productoId;
        Codigo = codigo;
        Descripcion = descripcion;
        Cantidad = cantidad;
        CostoUnitario = costoUnitario;
    }

    public Guid RecepcionId { get; private set; }
    public Guid PreRecepcionItemId { get; private set; }
    public Guid ProductoId { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public decimal Cantidad { get; private set; }
    public decimal? CostoUnitario { get; private set; }
}
