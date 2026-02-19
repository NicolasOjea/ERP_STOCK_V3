using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class PromocionCategoria : EntityBase
{
    private PromocionCategoria()
    {
    }

    public PromocionCategoria(
        Guid id,
        Guid tenantId,
        Guid promocionId,
        Guid categoriaId,
        DateTimeOffset createdAtUtc)
        : base(id, tenantId, createdAtUtc)
    {
        if (promocionId == Guid.Empty) throw new ArgumentException("PromocionId is required.", nameof(promocionId));
        if (categoriaId == Guid.Empty) throw new ArgumentException("CategoriaId is required.", nameof(categoriaId));

        PromocionId = promocionId;
        CategoriaId = categoriaId;
    }

    public Guid PromocionId { get; private set; }
    public Guid CategoriaId { get; private set; }
}
