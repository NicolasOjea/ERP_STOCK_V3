using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Pos.Application.DTOs.Etiquetas;
using Pos.Application.DTOs.Products;
using Pos.Domain.Enums;
using Xunit;

namespace Pos.Tests;

public sealed class CodigosBarraPdfTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public CodigosBarraPdfTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Codigos_barra_endpoint_devuelve_pdf_agrupado_por_proveedor()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(
            PermissionCodes.ProductoEditar,
            PermissionCodes.ProductoVer);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var proveedorA = await TestData.CreateProveedorAsync(client, $"ProveedorA-{Guid.NewGuid():N}");
        var proveedorB = await TestData.CreateProveedorAsync(client, $"ProveedorB-{Guid.NewGuid():N}");

        var productoAName = $"ProductoA-{Guid.NewGuid():N}";
        var productoBName = $"ProductoB-{Guid.NewGuid():N}";
        var productoA = await CreateProducto(client, productoAName, $"SKUA-{Guid.NewGuid():N}".Substring(0, 12), proveedorA);
        var productoB = await CreateProducto(client, productoBName, $"SKUB-{Guid.NewGuid():N}".Substring(0, 12), proveedorB);

        var request = new CodigosBarraRequestDto(new[] { productoA.Id, productoB.Id });
        var response = await client.PostAsJsonAsync("/api/v1/etiquetas/codigos-barra/pdf", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        Assert.NotEmpty(bytes);

        var pdfText = Encoding.Latin1.GetString(bytes);
        Assert.Contains(productoAName, pdfText, StringComparison.Ordinal);
        Assert.Contains(productoBName, pdfText, StringComparison.Ordinal);
    }

    private static async Task<ProductDetailDto> CreateProducto(HttpClient client, string name, string sku, Guid proveedorId)
    {
        var response = await client.PostAsJsonAsync("/api/v1/productos", new ProductCreateDto(
            name,
            sku,
            null,
            null,
            proveedorId,
            true));

        response.EnsureSuccessStatusCode();
        var detail = await response.Content.ReadFromJsonAsync<ProductDetailDto>();
        return detail!;
    }
}
