using System.Net;
using System.Net.Http.Json;
using Pos.Application.DTOs.ListasPrecio;
using Pos.Application.DTOs.Products;
using Pos.Application.DTOs.Ventas;
using Pos.Domain.Enums;
using Xunit;

namespace Pos.Tests;

public sealed class ListaPrecioTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public ListaPrecioTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Lista_aplicada_correctamente()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(
            PermissionCodes.ProductoEditar,
            PermissionCodes.ProductoVer,
            PermissionCodes.VentaCrear);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        await TestData.OpenCajaSessionAsync(client);

        var listResponse = await client.GetAsync("/api/v1/listas-precio");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var listas = await listResponse.Content.ReadFromJsonAsync<List<ListaPrecioDto>>();
        Assert.NotNull(listas);

        var lista = listas!.FirstOrDefault(l => l.Nombre == "Minorista");
        if (lista is null)
        {
            var createResponse = await client.PostAsJsonAsync("/api/v1/listas-precio", new ListaPrecioCreateDto("Minorista", true));
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            lista = await createResponse.Content.ReadFromJsonAsync<ListaPrecioDto>();
            Assert.NotNull(lista);
        }

        var proveedorId = await TestData.CreateProveedorAsync(client);
        var sku = $"SKU-{Guid.NewGuid():N}";
        var productResponse = await client.PostAsJsonAsync("/api/v1/productos", new ProductCreateDto(
            $"Producto {Guid.NewGuid():N}",
            sku,
            null,
            null,
            proveedorId,
            true,
            1m));

        Assert.Equal(HttpStatusCode.Created, productResponse.StatusCode);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductDetailDto>();
        Assert.NotNull(product);

        var codigo = $"COD-{Guid.NewGuid():N}";
        var addCode = await client.PostAsJsonAsync(
            $"/api/v1/productos/{product!.Id}/codigos",
            new ProductCodeCreateDto(codigo));
        Assert.Equal(HttpStatusCode.Created, addCode.StatusCode);

        var itemsResponse = await client.PutAsJsonAsync(
            $"/api/v1/listas-precio/{lista!.Id}/items",
            new ListaPrecioItemsUpdateDto(new[]
            {
                new ListaPrecioItemUpsertDto(product.Id, 7.5m)
            }));
        Assert.Equal(HttpStatusCode.NoContent, itemsResponse.StatusCode);

        var ventaResponse = await client.PostAsync("/api/v1/ventas", null);
        Assert.Equal(HttpStatusCode.Created, ventaResponse.StatusCode);
        var venta = await ventaResponse.Content.ReadFromJsonAsync<VentaDto>();
        Assert.NotNull(venta);

        var scanResponse = await client.PostAsJsonAsync(
            $"/api/v1/ventas/{venta!.Id}/items/scan",
            new VentaScanRequestDto(codigo));

        Assert.Equal(HttpStatusCode.OK, scanResponse.StatusCode);
        var item = await scanResponse.Content.ReadFromJsonAsync<VentaItemDto>();
        Assert.NotNull(item);
        Assert.Equal(7.5m, item!.PrecioUnitario);
        Assert.Equal(7.5m, item.Subtotal);
    }
}
