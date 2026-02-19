using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Pos.Application.DTOs.Importaciones;
using Pos.Application.DTOs.Products;
using Pos.Domain.Enums;
using Xunit;

namespace Pos.Tests;

public sealed class ImportacionProductosTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public ImportacionProductosTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Preview_detecta_duplicados()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(
            PermissionCodes.ProductoEditar,
            PermissionCodes.ProductoVer);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var csv = new StringBuilder()
            .AppendLine("sku,nombre,codigo,precio_base,activo")
            .AppendLine("SKU-1,Prod Uno,CODE-1,10,true")
            .AppendLine("SKU-1,Prod Dos,CODE-2,12,true")
            .ToString();

        var content = BuildMultipart(csv);
        var response = await client.PostAsync("/api/v1/importaciones/productos/preview", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var preview = await response.Content.ReadFromJsonAsync<ProductImportPreviewDto>();
        Assert.NotNull(preview);
        Assert.True(preview!.ErrorCount > 0);
        Assert.Contains(preview.Rows, r => r.Errores.Contains("SKU duplicado en archivo."));
    }

    [Fact]
    public async Task Confirm_crea_productos()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(
            PermissionCodes.ProductoEditar,
            PermissionCodes.ProductoVer);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var sku1 = $"SKU-IMP-{Guid.NewGuid():N}";
        var sku2 = $"SKU-IMP-{Guid.NewGuid():N}";

        var csv = new StringBuilder()
            .AppendLine("sku,nombre,codigo,precio_base,activo")
            .AppendLine($"{sku1},Producto Importado 1,CODE-IMP-1,5,true")
            .AppendLine($"{sku2},Producto Importado 2,CODE-IMP-2,7,true")
            .ToString();

        var confirmContent = BuildMultipart(csv);
        var confirmResponse = await client.PostAsync("/api/v1/importaciones/productos/confirm", confirmContent);

        Assert.Equal(HttpStatusCode.OK, confirmResponse.StatusCode);
        var result = await confirmResponse.Content.ReadFromJsonAsync<ProductImportConfirmResultDto>();
        Assert.NotNull(result);
        Assert.Equal(2, result!.Created);

        var listResponse = await client.GetAsync($"/api/v1/productos?search={sku1}");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var products = await listResponse.Content.ReadFromJsonAsync<List<ProductListItemDto>>();
        Assert.NotNull(products);
        Assert.Contains(products!, p => p.Sku == sku1);
    }

    private static MultipartFormDataContent BuildMultipart(string csv)
    {
        var content = new MultipartFormDataContent();
        var bytes = Encoding.UTF8.GetBytes(csv);
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
        content.Add(fileContent, "file", "productos.csv");
        return content;
    }
}
