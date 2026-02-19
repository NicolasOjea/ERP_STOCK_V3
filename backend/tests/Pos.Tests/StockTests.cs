using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pos.Application.DTOs.Products;
using Pos.Application.DTOs.Stock;
using Pos.Domain.Enums;
using Pos.Infrastructure.Persistence;
using Xunit;

namespace Pos.Tests;

public sealed class StockTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public StockTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Update_stock_config_creates_or_updates()
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

        var patchRequest = new HttpRequestMessage(HttpMethod.Patch, $"/api/v1/productos/{created!.Id}/stock-config")
        {
            Content = JsonContent.Create(new StockConfigUpdateDto(5m, 8m, 50m))
        };
        var updateResponse = await client.SendAsync(patchRequest);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var config = await updateResponse.Content.ReadFromJsonAsync<StockConfigDto>();
        Assert.NotNull(config);
        Assert.Equal(5m, config!.StockMinimo);
        Assert.Equal(8m, config.StockDeseado);
        Assert.Equal(50m, config.ToleranciaPct);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PosDbContext>();
        var entity = await db.ProductoStockConfigs.AsNoTracking()
            .SingleOrDefaultAsync(c => c.ProductoId == created.Id && c.SucursalId == SeedData.SucursalId);

        Assert.NotNull(entity);
        Assert.Equal(5m, entity!.StockMinimo);
    }

    [Fact]
    public async Task Get_saldos_creates_missing_with_zero()
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

        var saldosResponse = await client.GetAsync($"/api/v1/stock/saldos?search={sku}");
        Assert.Equal(HttpStatusCode.OK, saldosResponse.StatusCode);

        var saldos = await saldosResponse.Content.ReadFromJsonAsync<List<StockSaldoDto>>();
        Assert.NotNull(saldos);
        var saldo = saldos!.Single(s => s.ProductoId == created!.Id);
        Assert.Equal(0m, saldo.CantidadActual);
    }
}
