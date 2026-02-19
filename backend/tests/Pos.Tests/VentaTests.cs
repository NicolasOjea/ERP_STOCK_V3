using System.Net;
using System.Net.Http.Json;
using Pos.Application.DTOs.Products;
using Pos.Application.DTOs.Ventas;
using Pos.Domain.Enums;
using Xunit;

namespace Pos.Tests;

public sealed class VentaTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public VentaTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Scan_agrega_item()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(
            PermissionCodes.ProductoEditar,
            PermissionCodes.ProductoVer,
            PermissionCodes.VentaCrear);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var code = $"CODE-{Guid.NewGuid():N}";
        await TestData.OpenCajaSessionAsync(client);
        var venta = await CrearVentaConProductoAsync(client, code);

        var scanResponse = await client.PostAsJsonAsync(
            $"/api/v1/ventas/{venta.Id}/items/scan",
            new VentaScanRequestDto(code));

        Assert.Equal(HttpStatusCode.OK, scanResponse.StatusCode);

        var ventaResponse = await client.GetAsync($"/api/v1/ventas/{venta.Id}");
        Assert.Equal(HttpStatusCode.OK, ventaResponse.StatusCode);
        var ventaActual = await ventaResponse.Content.ReadFromJsonAsync<VentaDto>();

        Assert.NotNull(ventaActual);
        Assert.Single(ventaActual!.Items);
        Assert.Equal(1m, ventaActual.Items.First().Cantidad);
    }

    [Fact]
    public async Task Scan_repetido_incrementa_cantidad()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(
            PermissionCodes.ProductoEditar,
            PermissionCodes.ProductoVer,
            PermissionCodes.VentaCrear);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var code = $"CODE-{Guid.NewGuid():N}";
        await TestData.OpenCajaSessionAsync(client);
        var venta = await CrearVentaConProductoAsync(client, code);

        var first = await client.PostAsJsonAsync(
            $"/api/v1/ventas/{venta.Id}/items/scan",
            new VentaScanRequestDto(code));
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        var second = await client.PostAsJsonAsync(
            $"/api/v1/ventas/{venta.Id}/items/scan",
            new VentaScanRequestDto(code));
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);

        var ventaResponse = await client.GetAsync($"/api/v1/ventas/{venta.Id}");
        Assert.Equal(HttpStatusCode.OK, ventaResponse.StatusCode);
        var ventaActual = await ventaResponse.Content.ReadFromJsonAsync<VentaDto>();

        Assert.NotNull(ventaActual);
        var item = Assert.Single(ventaActual!.Items);
        Assert.Equal(2m, item.Cantidad);
    }

    [Fact]
    public async Task Scan_invalido_retorna_404()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(
            PermissionCodes.ProductoEditar,
            PermissionCodes.ProductoVer,
            PermissionCodes.VentaCrear);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        await TestData.OpenCajaSessionAsync(client);
        var ventaResponse = await client.PostAsync("/api/v1/ventas", null);
        Assert.Equal(HttpStatusCode.Created, ventaResponse.StatusCode);
        var venta = await ventaResponse.Content.ReadFromJsonAsync<VentaDto>();
        Assert.NotNull(venta);

        var scanResponse = await client.PostAsJsonAsync(
            $"/api/v1/ventas/{venta!.Id}/items/scan",
            new VentaScanRequestDto("NO-EXISTE"));

        Assert.Equal(HttpStatusCode.NotFound, scanResponse.StatusCode);
    }

    private static async Task<VentaDto> CrearVentaConProductoAsync(HttpClient client, string code)
    {
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
        var product = await createResponse.Content.ReadFromJsonAsync<ProductDetailDto>();
        Assert.NotNull(product);

        var addCodeResponse = await client.PostAsJsonAsync(
            $"/api/v1/productos/{product!.Id}/codigos",
            new ProductCodeCreateDto(code));

        Assert.Equal(HttpStatusCode.Created, addCodeResponse.StatusCode);

        var ventaResponse = await client.PostAsync("/api/v1/ventas", null);
        Assert.Equal(HttpStatusCode.Created, ventaResponse.StatusCode);
        var venta = await ventaResponse.Content.ReadFromJsonAsync<VentaDto>();
        Assert.NotNull(venta);
        return venta!;
    }
}
