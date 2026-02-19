using System.Net;
using System.Net.Http.Json;
using Pos.Application.DTOs.Products;
using Pos.Domain.Enums;
using Xunit;

namespace Pos.Tests;

public sealed class ProductTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public ProductTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_product_returns_created()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(PermissionCodes.ProductoEditar, PermissionCodes.ProductoVer);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var proveedorId = await TestData.CreateProveedorAsync(client);
        var sku = $"SKU-{Guid.NewGuid():N}";
        var response = await client.PostAsJsonAsync("/api/v1/productos", new ProductCreateDto(
            $"Producto {Guid.NewGuid():N}",
            sku,
            null,
            null,
            proveedorId,
            true));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ProductDetailDto>();
        Assert.NotNull(payload);
        Assert.Equal(sku, payload!.Sku);
    }

    [Fact]
    public async Task Add_code_and_search_by_code()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(PermissionCodes.ProductoEditar, PermissionCodes.ProductoVer);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var proveedorId = await TestData.CreateProveedorAsync(client);
        var sku = $"SKU-{Guid.NewGuid():N}";
        var createResponse = await client.PostAsJsonAsync("/api/v1/productos", new ProductCreateDto(
            $"Producto {Guid.NewGuid():N}",
            sku,
            null,
            null,
            proveedorId,
            true));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<ProductDetailDto>();
        Assert.NotNull(created);

        var code = $"CODE-{Guid.NewGuid():N}";
        var addCodeResponse = await client.PostAsJsonAsync(
            $"/api/v1/productos/{created!.Id}/codigos",
            new ProductCodeCreateDto(code));

        Assert.Equal(HttpStatusCode.Created, addCodeResponse.StatusCode);

        var listResponse = await client.GetAsync($"/api/v1/productos?search={code}");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var list = await listResponse.Content.ReadFromJsonAsync<List<ProductListItemDto>>();
        Assert.NotNull(list);
        Assert.Contains(list!, item => item.Id == created.Id);
    }

    [Fact]
    public async Task Duplicate_code_returns_409()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(PermissionCodes.ProductoEditar, PermissionCodes.ProductoVer);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var proveedorId = await TestData.CreateProveedorAsync(client);
        var sku = $"SKU-{Guid.NewGuid():N}";
        var createResponse = await client.PostAsJsonAsync("/api/v1/productos", new ProductCreateDto(
            $"Producto {Guid.NewGuid():N}",
            sku,
            null,
            null,
            proveedorId,
            true));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<ProductDetailDto>();
        Assert.NotNull(created);

        var code = $"CODE-{Guid.NewGuid():N}";
        var first = await client.PostAsJsonAsync(
            $"/api/v1/productos/{created!.Id}/codigos",
            new ProductCodeCreateDto(code));

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var duplicate = await client.PostAsJsonAsync(
            $"/api/v1/productos/{created.Id}/codigos",
            new ProductCodeCreateDto(code));

        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
    }
}
