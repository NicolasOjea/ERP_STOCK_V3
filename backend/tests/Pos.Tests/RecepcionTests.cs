using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pos.Application.DTOs.DocumentosCompra;
using Pos.Application.DTOs.PreRecepciones;
using Pos.Application.DTOs.Products;
using Pos.Domain.Enums;
using Pos.Infrastructure.Persistence;
using Xunit;

namespace Pos.Tests;

public sealed class RecepcionTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public RecepcionTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Confirmar_con_item_no_encontrado_devuelve_409()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(PermissionCodes.ComprasRegistrar);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var parsePayload = new
        {
            proveedorId = (string?)null,
            numero = $"REM-{Guid.NewGuid():N}",
            fecha = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            items = new[]
            {
                new { codigo = $"X-{Guid.NewGuid():N}", descripcion = "Item", cantidad = 1m }
            }
        };

        var parseResponse = await client.PostAsJsonAsync("/api/v1/documentos-compra/parse", parsePayload);
        Assert.Equal(HttpStatusCode.Created, parseResponse.StatusCode);
        var parseResult = await parseResponse.Content.ReadFromJsonAsync<DocumentoCompraParseResultDto>();
        Assert.NotNull(parseResult);

        var preResponse = await client.PostAsJsonAsync("/api/v1/pre-recepciones", new PreRecepcionCreateDto(
            parseResult!.DocumentoCompraId));
        Assert.Equal(HttpStatusCode.Created, preResponse.StatusCode);
        var preRecepcion = await preResponse.Content.ReadFromJsonAsync<PreRecepcionDto>();
        Assert.NotNull(preRecepcion);

        var confirmResponse = await client.PostAsync($"/api/v1/pre-recepciones/{preRecepcion!.Id}/confirmar", content: null);
        Assert.Equal(HttpStatusCode.Conflict, confirmResponse.StatusCode);
    }

    [Fact]
    public async Task Confirmar_ok_incrementa_stock()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(
            PermissionCodes.ComprasRegistrar,
            PermissionCodes.ProductoEditar,
            PermissionCodes.ProductoVer);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var proveedorId = await TestData.CreateProveedorAsync(client);
        var sku = $"SKU-{Guid.NewGuid():N}";
        var productResponse = await client.PostAsJsonAsync("/api/v1/productos", new ProductCreateDto(
            $"Producto {Guid.NewGuid():N}",
            sku,
            null,
            null,
            proveedorId,
            true));

        Assert.Equal(HttpStatusCode.Created, productResponse.StatusCode);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductDetailDto>();
        Assert.NotNull(product);

        var codigo = $"COD-{Guid.NewGuid():N}";
        var addCode = await client.PostAsJsonAsync(
            $"/api/v1/productos/{product!.Id}/codigos",
            new ProductCodeCreateDto(codigo));
        Assert.Equal(HttpStatusCode.Created, addCode.StatusCode);

        var parsePayload = new
        {
            proveedorId = (string?)null,
            numero = $"REM-{Guid.NewGuid():N}",
            fecha = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            items = new[]
            {
                new { codigo, descripcion = "Item", cantidad = 3m }
            }
        };

        var parseResponse = await client.PostAsJsonAsync("/api/v1/documentos-compra/parse", parsePayload);
        Assert.Equal(HttpStatusCode.Created, parseResponse.StatusCode);
        var parseResult = await parseResponse.Content.ReadFromJsonAsync<DocumentoCompraParseResultDto>();
        Assert.NotNull(parseResult);

        var preResponse = await client.PostAsJsonAsync("/api/v1/pre-recepciones", new PreRecepcionCreateDto(
            parseResult!.DocumentoCompraId));
        Assert.Equal(HttpStatusCode.Created, preResponse.StatusCode);
        var preRecepcion = await preResponse.Content.ReadFromJsonAsync<PreRecepcionDto>();
        Assert.NotNull(preRecepcion);

        var confirmResponse = await client.PostAsync($"/api/v1/pre-recepciones/{preRecepcion!.Id}/confirmar", content: null);
        Assert.Equal(HttpStatusCode.OK, confirmResponse.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PosDbContext>();
        var saldo = await db.StockSaldos.AsNoTracking()
            .FirstOrDefaultAsync(s => s.TenantId == SeedData.TenantId && s.SucursalId == SeedData.SucursalId && s.ProductoId == product.Id);

        Assert.NotNull(saldo);
        Assert.Equal(3m, saldo!.CantidadActual);
    }
}
