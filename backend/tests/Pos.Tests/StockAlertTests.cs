using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Pos.Application.DTOs.Products;
using Pos.Application.DTOs.Stock;
using Pos.Domain.Enums;
using Xunit;

namespace Pos.Tests;

public sealed class StockAlertTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public StockAlertTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Alertas_critico_y_bajo_correctas()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(
            PermissionCodes.ProductoEditar,
            PermissionCodes.ProductoVer,
            PermissionCodes.StockAjustar);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var proveedorId = await TestData.CreateProveedorAsync(client);
        var skuCritico = $"SKU-CRIT-{Guid.NewGuid():N}";
        var criticoResponse = await client.PostAsJsonAsync("/api/v1/productos", new ProductCreateDto(
            $"Producto Critico {Guid.NewGuid():N}",
            skuCritico,
            null,
            null,
            proveedorId,
            true));

        Assert.Equal(HttpStatusCode.Created, criticoResponse.StatusCode);
        var critico = await criticoResponse.Content.ReadFromJsonAsync<ProductDetailDto>();
        Assert.NotNull(critico);

        var skuBajo = $"SKU-BAJO-{Guid.NewGuid():N}";
        var bajoResponse = await client.PostAsJsonAsync("/api/v1/productos", new ProductCreateDto(
            $"Producto Bajo {Guid.NewGuid():N}",
            skuBajo,
            null,
            null,
            proveedorId,
            true));

        Assert.Equal(HttpStatusCode.Created, bajoResponse.StatusCode);
        var bajo = await bajoResponse.Content.ReadFromJsonAsync<ProductDetailDto>();
        Assert.NotNull(bajo);

        var patchCritico = new HttpRequestMessage(HttpMethod.Patch, $"/api/v1/productos/{critico!.Id}/stock-config")
        {
            Content = JsonContent.Create(new StockConfigUpdateDto(10m, 15m, 25m))
        };
        var patchCriticoResp = await client.SendAsync(patchCritico);
        Assert.Equal(HttpStatusCode.OK, patchCriticoResp.StatusCode);

        var patchBajo = new HttpRequestMessage(HttpMethod.Patch, $"/api/v1/productos/{bajo!.Id}/stock-config")
        {
            Content = JsonContent.Create(new StockConfigUpdateDto(10m, 15m, 25m))
        };
        var patchBajoResp = await client.SendAsync(patchBajo);
        Assert.Equal(HttpStatusCode.OK, patchBajoResp.StatusCode);

        var ingreso = new StockMovimientoCreateDto(
            "AJUSTE",
            "Ingreso",
            new[]
            {
                new StockMovimientoItemCreateDto(bajo.Id, 11m, true)
            });

        var ingresoResponse = await client.PostAsJsonAsync("/api/v1/stock/ajustes", ingreso);
        Assert.Equal(HttpStatusCode.Created, ingresoResponse.StatusCode);

        var alertasResponse = await client.GetAsync("/api/v1/stock/alertas");
        Assert.Equal(HttpStatusCode.OK, alertasResponse.StatusCode);

        var alertas = await alertasResponse.Content.ReadFromJsonAsync<List<StockAlertaDto>>();
        Assert.NotNull(alertas);

        var criticoAlert = alertas!.Single(a => a.ProductoId == critico.Id);
        var bajoAlert = alertas.Single(a => a.ProductoId == bajo.Id);

        Assert.Equal("CRITICO", criticoAlert.Nivel);
        Assert.Equal("BAJO", bajoAlert.Nivel);
    }
}
