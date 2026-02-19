using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pos.Application.DTOs.Caja;
using Pos.Application.DTOs.Products;
using Pos.Application.DTOs.Stock;
using Pos.Application.DTOs.Ventas;
using Pos.Domain.Entities;
using Pos.Domain.Enums;
using Pos.Infrastructure.Persistence;
using Xunit;

namespace Pos.Tests;

public sealed class VentaAnularTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public VentaAnularTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Anular_sin_permiso_retorna_403()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var setupClient = _factory.CreateClient();
        var setupToken = _factory.CreateTokenWithPermissions(
            PermissionCodes.ProductoEditar,
            PermissionCodes.ProductoVer,
            PermissionCodes.VentaCrear,
            PermissionCodes.VentaConfirmar,
            PermissionCodes.StockAjustar,
            PermissionCodes.CajaMovimiento);
        setupClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", setupToken);

        var (venta, productoId) = await CrearVentaConfirmadaAsync(setupClient);

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(PermissionCodes.VentaCrear);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync(
            $"/api/v1/ventas/{venta.Id}/anular",
            new VentaAnularRequestDto("Cliente cancelo"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Anular_ok_revierte_stock_y_caja()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(
            PermissionCodes.ProductoEditar,
            PermissionCodes.ProductoVer,
            PermissionCodes.VentaCrear,
            PermissionCodes.VentaConfirmar,
            PermissionCodes.VentaAnular,
            PermissionCodes.StockAjustar,
            PermissionCodes.CajaMovimiento);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var (venta, productoId) = await CrearVentaConfirmadaAsync(client);

        var anularResponse = await client.PostAsJsonAsync(
            $"/api/v1/ventas/{venta.Id}/anular",
            new VentaAnularRequestDto("Error de cobro"));

        Assert.Equal(HttpStatusCode.OK, anularResponse.StatusCode);

        var saldosResponse = await client.GetAsync($"/api/v1/stock/saldos?search={venta.Items.First().Sku}");
        Assert.Equal(HttpStatusCode.OK, saldosResponse.StatusCode);
        var saldos = await saldosResponse.Content.ReadFromJsonAsync<List<StockSaldoDto>>();
        Assert.NotNull(saldos);
        var saldo = saldos!.Single(s => s.ProductoId == productoId);
        Assert.Equal(5m, saldo.CantidadActual);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PosDbContext>();
        var stockMov = await db.StockMovimientos.AsNoTracking()
            .FirstOrDefaultAsync(m => m.TenantId == SeedData.TenantId && m.Tipo == StockMovimientoTipo.EntradaAnulacion);
        Assert.NotNull(stockMov);

        var cajaMov = await db.CajaMovimientos.AsNoTracking()
            .FirstOrDefaultAsync(m => m.TenantId == SeedData.TenantId && m.Tipo == CajaMovimientoTipo.Egreso);
        Assert.NotNull(cajaMov);
    }

    private async Task<(VentaDto Venta, Guid ProductoId)> CrearVentaConfirmadaAsync(HttpClient client)
    {
        var code = $"CODE-{Guid.NewGuid():N}";
        var sku = $"SKU-{Guid.NewGuid():N}";
        var proveedorId = await TestData.CreateProveedorAsync(client);

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

        var cajaId = await CrearCajaAsync();
        var abrirResponse = await client.PostAsJsonAsync(
            "/api/v1/caja/sesiones/abrir",
            new CajaSesionAbrirDto(cajaId, 0m));
        Assert.Equal(HttpStatusCode.Created, abrirResponse.StatusCode);

        var scan = await client.PostAsJsonAsync(
            $"/api/v1/ventas/{venta!.Id}/items/scan",
            new VentaScanRequestDto(code));
        Assert.Equal(HttpStatusCode.OK, scan.StatusCode);

        var ingreso = new StockMovimientoCreateDto(
            "AJUSTE",
            "Ingreso inicial",
            new[]
            {
                new StockMovimientoItemCreateDto(product!.Id, 5m, true)
            });
        var ingresoResponse = await client.PostAsJsonAsync("/api/v1/stock/ajustes", ingreso);
        Assert.Equal(HttpStatusCode.Created, ingresoResponse.StatusCode);

        var confirm = await client.PostAsJsonAsync(
            $"/api/v1/ventas/{venta.Id}/confirmar",
            new VentaConfirmRequestDto(new[]
            {
                new VentaPagoRequestDto("EFECTIVO", 1m)
            }));
        Assert.Equal(HttpStatusCode.OK, confirm.StatusCode);

        var updated = await client.GetAsync($"/api/v1/ventas/{venta.Id}");
        Assert.Equal(HttpStatusCode.OK, updated.StatusCode);
        var ventaActual = await updated.Content.ReadFromJsonAsync<VentaDto>();
        Assert.NotNull(ventaActual);

        return (ventaActual!, product!.Id);
    }

    private async Task<Guid> CrearCajaAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PosDbContext>();
        var caja = new Caja(
            Guid.NewGuid(),
            SeedData.TenantId,
            SeedData.SucursalId,
            $"Caja {Guid.NewGuid():N}",
            Random.Shared.Next(1000, 999999).ToString(),
            DateTimeOffset.UtcNow);
        db.Cajas.Add(caja);
        await db.SaveChangesAsync();
        return caja.Id;
    }
}
