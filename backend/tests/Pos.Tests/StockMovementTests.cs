using System.Net;
using System.Net.Http.Json;
using Pos.Application.DTOs.Products;
using Pos.Application.DTOs.Stock;
using Pos.Domain.Enums;
using Xunit;

namespace Pos.Tests;

public sealed class StockMovementTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public StockMovementTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Ajuste_negativo_rechaza()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(
            PermissionCodes.ProductoEditar,
            PermissionCodes.ProductoVer,
            PermissionCodes.StockAjustar);
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

        var ajuste = new StockMovimientoCreateDto(
            "AJUSTE",
            "Baja sin stock",
            new[]
            {
                new StockMovimientoItemCreateDto(created!.Id, 5m, false)
            });

        var response = await client.PostAsJsonAsync("/api/v1/stock/ajustes", ajuste);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Merma_ok_descuenta()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(
            PermissionCodes.ProductoEditar,
            PermissionCodes.ProductoVer,
            PermissionCodes.StockAjustar);
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

        var ingreso = new StockMovimientoCreateDto(
            "AJUSTE",
            "Ingreso inicial",
            new[]
            {
                new StockMovimientoItemCreateDto(created!.Id, 10m, true)
            });

        var ingresoResponse = await client.PostAsJsonAsync("/api/v1/stock/ajustes", ingreso);
        Assert.Equal(HttpStatusCode.Created, ingresoResponse.StatusCode);

        var merma = new StockMovimientoCreateDto(
            "MERMA",
            "Merma",
            new[]
            {
                new StockMovimientoItemCreateDto(created.Id, 3m, false)
            });

        var mermaResponse = await client.PostAsJsonAsync("/api/v1/stock/ajustes", merma);
        Assert.Equal(HttpStatusCode.Created, mermaResponse.StatusCode);

        var saldosResponse = await client.GetAsync($"/api/v1/stock/saldos?search={sku}");
        Assert.Equal(HttpStatusCode.OK, saldosResponse.StatusCode);

        var saldos = await saldosResponse.Content.ReadFromJsonAsync<List<StockSaldoDto>>();
        Assert.NotNull(saldos);
        var saldo = saldos!.Single(s => s.ProductoId == created.Id);
        Assert.Equal(7m, saldo.CantidadActual);
    }
}
