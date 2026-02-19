using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pos.Application.DTOs.Products;
using Pos.Application.DTOs.Proveedores;
using Pos.Domain.Enums;
using Pos.Infrastructure.Persistence;
using Xunit;

namespace Pos.Tests;

public sealed class ProveedorPrincipalTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public ProveedorPrincipalTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Marcar_nuevo_principal_desmarca_anterior()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(
            PermissionCodes.ProductoEditar,
            PermissionCodes.ProveedorGestionar);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var proveedorAResponse = await client.PostAsJsonAsync("/api/v1/proveedores", new ProveedorCreateDto(
            $"Proveedor A {Guid.NewGuid():N}",
            "1133445566",
            null,
            null,
            true));
        Assert.Equal(HttpStatusCode.Created, proveedorAResponse.StatusCode);
        var proveedorA = await proveedorAResponse.Content.ReadFromJsonAsync<ProveedorDto>();
        Assert.NotNull(proveedorA);

        var proveedorBResponse = await client.PostAsJsonAsync("/api/v1/proveedores", new ProveedorCreateDto(
            $"Proveedor B {Guid.NewGuid():N}",
            "1133445566",
            null,
            null,
            true));
        Assert.Equal(HttpStatusCode.Created, proveedorBResponse.StatusCode);
        var proveedorB = await proveedorBResponse.Content.ReadFromJsonAsync<ProveedorDto>();
        Assert.NotNull(proveedorB);

        var sku = $"SKU-{Guid.NewGuid():N}";
        var productResponse = await client.PostAsJsonAsync("/api/v1/productos", new ProductCreateDto(
            $"Producto {Guid.NewGuid():N}",
            sku,
            null,
            null,
            proveedorA!.Id,
            true));
        Assert.Equal(HttpStatusCode.Created, productResponse.StatusCode);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductDetailDto>();
        Assert.NotNull(product);

        var relationAResponse = await client.PostAsJsonAsync(
            $"/api/v1/productos/{product!.Id}/proveedores",
            new ProductProveedorCreateDto(proveedorA!.Id, true));
        Assert.Equal(HttpStatusCode.Created, relationAResponse.StatusCode);

        var relationBResponse = await client.PostAsJsonAsync(
            $"/api/v1/productos/{product.Id}/proveedores",
            new ProductProveedorCreateDto(proveedorB!.Id, false));
        Assert.Equal(HttpStatusCode.Created, relationBResponse.StatusCode);
        var relationB = await relationBResponse.Content.ReadFromJsonAsync<ProductProveedorDto>();
        Assert.NotNull(relationB);

        var patchRequest = new HttpRequestMessage(HttpMethod.Patch, $"/api/v1/productos/{product.Id}/proveedores/{relationB!.Id}")
        {
            Content = JsonContent.Create(new ProductProveedorUpdateDto(true))
        };
        var patchResponse = await client.SendAsync(patchRequest);
        Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PosDbContext>();
        var relations = await db.ProductoProveedores.AsNoTracking()
            .Where(r => r.TenantId == SeedData.TenantId && r.ProductoId == product.Id)
            .ToListAsync();

        Assert.Equal(2, relations.Count);
        var principal = relations.Single(r => r.EsPrincipal);
        Assert.Equal(proveedorB.Id, principal.ProveedorId);

        var previousPrincipal = relations.Single(r => r.ProveedorId == proveedorA.Id);
        Assert.False(previousPrincipal.EsPrincipal);

        var productEntity = await db.Productos.AsNoTracking()
            .SingleAsync(p => p.TenantId == SeedData.TenantId && p.Id == product.Id);
        Assert.Equal(proveedorB.Id, productEntity.ProveedorId);
    }
}
