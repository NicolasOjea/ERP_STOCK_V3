using Pos.Application.DTOs.Pricing;
using Pos.Domain.Enums;

namespace Pos.Application.UseCases.Pricing;

public interface IPromoStrategy
{
    PromocionTipo Tipo { get; }

    PromoApplyResult Apply(PromocionAplicableDto promo, PricingContext context);
}
