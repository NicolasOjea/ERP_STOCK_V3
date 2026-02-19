using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class ProductoCodigo : EntityBase
{
    private ProductoCodigo()
    {
    }

    public ProductoCodigo(
        Guid id,
        Guid tenantId,
        Guid productoId,
        string codigo,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (productoId == Guid.Empty) throw new ArgumentException("ProductoId is required.", nameof(productoId));
        if (string.IsNullOrWhiteSpace(codigo)) throw new ArgumentException("Codigo is required.", nameof(codigo));

        ProductoId = productoId;
        Codigo = codigo;
    }

    public Guid ProductoId { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
}
