using Pos.Application.DTOs.Pricing;
using Pos.Application.UseCases.Pricing;
using Pos.Application.UseCases.Pricing.Strategies;
using Pos.Domain.Enums;
using Xunit;

namespace Pos.Tests;

public sealed class PricingServiceTests
{
    [Fact]
    public void Dos_por_uno_aplica_gratis_el_mas_barato()
    {
        var categoriaId = Guid.NewGuid();
        var productoA = Guid.NewGuid();
        var productoB = Guid.NewGuid();
        var productoC = Guid.NewGuid();

        var venta = new VentaPricingSnapshotDto(
            Guid.NewGuid(),
            "Minorista",
            new List<VentaPricingItemDto>
            {
                new(Guid.NewGuid(), productoA, categoriaId, "A", "A", 1m, 10m),
                new(Guid.NewGuid(), productoB, categoriaId, "B", "B", 1m, 5m),
                new(Guid.NewGuid(), productoC, categoriaId, "C", "C", 1m, 8m)
            });

        var promo = new PromocionAplicableDto(
            Guid.NewGuid(),
            "2x1 Categoria",
            PromocionTipo.DosPorUno,
            100m,
            10,
            new[] { categoriaId });

        var service = CreateService();

        var result = service.Recalcular(venta, new[] { promo });

        Assert.Equal(23m, result.TotalBruto);
        Assert.Equal(5m, result.TotalDescuento);
        Assert.Equal(18m, result.TotalNeto);

        var itemBarato = result.Items.Single(i => i.ProductoId == productoB);
        Assert.Equal(5m, itemBarato.Descuento);
    }

    [Fact]
    public void Combo_aplica_cuando_estan_todos_los_items_requeridos()
    {
        var productoA = Guid.NewGuid();
        var productoB = Guid.NewGuid();

        var venta = new VentaPricingSnapshotDto(
            Guid.NewGuid(),
            "Minorista",
            new List<VentaPricingItemDto>
            {
                new(Guid.NewGuid(), productoA, null, "A", "A", 1m, 10m),
                new(Guid.NewGuid(), productoB, null, "B", "B", 1m, 20m)
            });

        var promo = new PromocionAplicableDto(
            Guid.NewGuid(),
            "Combo A+B",
            PromocionTipo.Combo,
            10m,
            5,
            Array.Empty<Guid>(),
            null,
            new List<PromoComboItemDto>
            {
                new(productoA, 1m),
                new(productoB, 1m)
            });

        var service = CreateService();

        var result = service.Recalcular(venta, new[] { promo });

        Assert.Equal(30m, result.TotalBruto);
        Assert.Equal(3m, result.TotalDescuento);
        Assert.Equal(27m, result.TotalNeto);

        var itemA = result.Items.Single(i => i.ProductoId == productoA);
        var itemB = result.Items.Single(i => i.ProductoId == productoB);
        Assert.Equal(1m, itemA.Descuento);
        Assert.Equal(2m, itemB.Descuento);
    }

    private static PricingService CreateService()
    {
        return new PricingService(new IPromoStrategy[]
        {
            new PorcCategoriaPromoStrategy(),
            new DosPorUnoPromoStrategy(),
            new ComboPromoStrategy()
        });
    }
}
