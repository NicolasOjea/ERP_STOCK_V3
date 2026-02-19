using System.Text;
using Pos.Application.DTOs.Stock;
using Pos.Infrastructure.Adapters.Pdf;
using Xunit;

namespace Pos.Tests;

public sealed class RemitoPdfGeneratorTests
{
    [Fact]
    public void Generate_returns_pdf_bytes_for_multiple_proveedores()
    {
        var generator = new RemitoPdfGenerator();
        var data = new StockRemitoPdfDataDto(
            DateTimeOffset.UtcNow,
            "R-TEST",
            new StockRemitoHeaderDto("Empresa Demo", "Sucursal Central"),
            new List<StockRemitoPdfProveedorDto>
            {
                new(
                    "Proveedor 1",
                    "1234",
                    "20-12345678-9",
                    "Calle 123",
                    new List<StockRemitoPdfItemDto>
                    {
                        new("Producto A", "SKU-A", 2m),
                        new("Producto B", "SKU-B", 5m)
                    }),
                new(
                    "Proveedor 2",
                    "5678",
                    null,
                    null,
                    new List<StockRemitoPdfItemDto>
                    {
                        new("Producto C", "SKU-C", 1m)
                    })
            });

        var bytes = generator.Generate(data);

        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 100);
        var header = Encoding.ASCII.GetString(bytes, 0, 4);
        Assert.Equal("%PDF", header);
    }
}
