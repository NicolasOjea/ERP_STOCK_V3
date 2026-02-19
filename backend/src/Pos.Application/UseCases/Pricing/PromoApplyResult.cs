namespace Pos.Application.UseCases.Pricing;

public sealed record PromoItemDiscount(Guid ItemId, decimal Descuento);

public sealed record PromoApplyResult(IReadOnlyList<PromoItemDiscount> ItemDiscounts)
{
    public static readonly PromoApplyResult Empty = new(Array.Empty<PromoItemDiscount>());
}
