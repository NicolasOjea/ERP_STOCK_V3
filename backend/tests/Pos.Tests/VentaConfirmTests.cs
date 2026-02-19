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

public sealed class VentaConfirmTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public VentaConfirmTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Confirmar_sin_stock_retorna_409()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(
            PermissionCodes.ProductoEditar,
            PermissionCodes.ProductoVer,
            PermissionCodes.VentaCrear,
            PermissionCodes.VentaConfirmar,
            PermissionCodes.CajaMovimiento);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var code = $"CODE-{Guid.NewGuid():N}";
        var (venta, productoId) = await CrearVentaConProductoAsync(client, code);

        var cajaId = await CrearCajaAsync();
        var abrirResponse = await client.PostAsJsonAsync(
            "/api/v1/caja/sesiones/abrir",
            new CajaSesionAbrirDto(cajaId, 0m));
        Assert.Equal(HttpStatusCode.Created, abrirResponse.StatusCode);

        var scan = await client.PostAsJsonAsync(
            $"/api/v1/ventas/{venta.Id}/items/scan",
            new VentaScanRequestDto(code));
        Assert.Equal(HttpStatusCode.OK, scan.StatusCode);

        var confirm = await client.PostAsJsonAsync(
            $"/api/v1/ventas/{venta.Id}/confirmar",
            new VentaConfirmRequestDto(new[]
            {
                new VentaPagoRequestDto("EFECTIVO", 1m)
            }));

        Assert.Equal(HttpStatusCode.Conflict, confirm.StatusCode);
    }

    [Fact]
    public async Task Confirmar_ok_descuenta_stock_y_crea_movimientos()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(
            PermissionCodes.ProductoEditar,
            PermissionCodes.ProductoVer,
            PermissionCodes.VentaCrear,
            PermissionCodes.VentaConfirmar,
            PermissionCodes.StockAjustar,
            PermissionCodes.CajaMovimiento);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var code = $"CODE-{Guid.NewGuid():N}";
        var (venta, productoId) = await CrearVentaConProductoAsync(client, code);

        var cajaId = await CrearCajaAsync();
        var abrirResponse = await client.PostAsJsonAsync(
            "/api/v1/caja/sesiones/abrir",
            new CajaSesionAbrirDto(cajaId, 0m));
        Assert.Equal(HttpStatusCode.Created, abrirResponse.StatusCode);

        var scan = await client.PostAsJsonAsync(
            $"/api/v1/ventas/{venta.Id}/items/scan",
            new VentaScanRequestDto(code));
        Assert.Equal(HttpStatusCode.OK, scan.StatusCode);

        var ingreso = new StockMovimientoCreateDto(
            "AJUSTE",
            "Ingreso inicial",
            new[]
            {
                new StockMovimientoItemCreateDto(productoId, 5m, true)
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

        var saldosResponse = await client.GetAsync($"/api/v1/stock/saldos?search={code}");
        Assert.Equal(HttpStatusCode.OK, saldosResponse.StatusCode);
        var saldos = await saldosResponse.Content.ReadFromJsonAsync<List<StockSaldoDto>>();
        Assert.NotNull(saldos);
        var saldo = saldos!.Single();
        Assert.Equal(4m, saldo.CantidadActual);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PosDbContext>();
        var movimiento = await db.StockMovimientos.AsNoTracking()
            .FirstOrDefaultAsync(m => m.TenantId == SeedData.TenantId && m.Tipo == StockMovimientoTipo.SalidaVenta);
        Assert.NotNull(movimiento);

        var cajaMov = await db.CajaMovimientos.AsNoTracking()
            .FirstOrDefaultAsync(m => m.TenantId == SeedData.TenantId && m.Tipo == CajaMovimientoTipo.Ingreso);
        Assert.NotNull(cajaMov);
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

    private static async Task<(VentaDto Venta, Guid ProductoId)> CrearVentaConProductoAsync(HttpClient client, string code)
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
        return (venta!, product!.Id);
    }
}
