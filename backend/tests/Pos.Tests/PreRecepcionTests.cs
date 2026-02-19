using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Pos.Application.DTOs.DocumentosCompra;
using Pos.Application.DTOs.PreRecepciones;
using Pos.Application.DTOs.Products;
using Pos.Domain.Enums;
using Pos.Domain.Entities;
using Pos.Infrastructure.Persistence;
using Xunit;

namespace Pos.Tests;

public sealed class PreRecepcionTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public PreRecepcionTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PreRecepcion_crea_items_con_match_ok()
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
                new { codigo, descripcion = "Item", cantidad = 1m }
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
        Assert.Single(preRecepcion!.Items);

        var item = preRecepcion.Items[0];
        Assert.Equal("OK", item.Estado);
        Assert.Equal(product.Id, item.ProductoId);
    }

    [Fact]
    public async Task PreRecepcion_no_cruza_tenant_en_match_codigo()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var codigo = $"COD-{Guid.NewGuid():N}";
        var otherTenantId = Guid.NewGuid();
        var otherProductId = Guid.NewGuid();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PosDbContext>();
            db.Tenants.Add(new Tenant(otherTenantId, $"Tenant {Guid.NewGuid():N}", DateTimeOffset.UtcNow));
            db.Productos.Add(new Producto(
                otherProductId,
                otherTenantId,
                $"Producto {Guid.NewGuid():N}",
                $"SKU-{Guid.NewGuid():N}",
                null,
                null,
                null,
                DateTimeOffset.UtcNow,
                100m,
                100m,
                true));
            db.ProductoCodigos.Add(new ProductoCodigo(Guid.NewGuid(), otherTenantId, otherProductId, codigo, DateTimeOffset.UtcNow));
            await db.SaveChangesAsync();
        }

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
                new { codigo, descripcion = "Item", cantidad = 1m }
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
        Assert.Single(preRecepcion!.Items);

        var item = preRecepcion.Items[0];
        Assert.Equal("NO_ENCONTRADO", item.Estado);
        Assert.Null(item.ProductoId);
    }
}
