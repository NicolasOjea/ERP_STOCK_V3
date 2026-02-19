using System.Text;
using Pos.Application.DTOs.Etiquetas;
using Pos.Infrastructure.Adapters.Pdf;
using Xunit;

namespace Pos.Tests;

public sealed class CodigoBarraPdfGeneratorTests
{
    [Fact]
    public void Generate_incluye_titulos_por_proveedor_y_productos()
    {
        var generator = new CodigoBarraPdfGenerator();
        var data = new CodigoBarraPdfDataDto(new List<CodigoBarraProveedorPdfDto>
        {
            new(
                "Proveedor Uno",
                new List<CodigoBarraItemPdfDto>
                {
                    new("Yerba", "YERBA-001"),
                    new("Azucar", "AZUCAR-002")
                }),
            new(
                "Proveedor Dos",
                new List<CodigoBarraItemPdfDto>
                {
                    new("Cafe", "CAFE-003")
                })
        });

        var bytes = generator.Generate(data);
        var raw = Encoding.Latin1.GetString(bytes);

        Assert.NotEmpty(bytes);
        Assert.Equal("%PDF", Encoding.ASCII.GetString(bytes, 0, 4));
        Assert.Contains("Proveedor Uno", raw, StringComparison.Ordinal);
        Assert.Contains("Proveedor Dos", raw, StringComparison.Ordinal);
        Assert.Contains("Yerba", raw, StringComparison.Ordinal);
        Assert.Contains("Cafe", raw, StringComparison.Ordinal);
        Assert.Contains("Pagina 1/", raw, StringComparison.Ordinal);
    }
}
