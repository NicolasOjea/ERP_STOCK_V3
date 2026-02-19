using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pos.Application.DTOs.Caja;
using Pos.Domain.Entities;
using Pos.Domain.Enums;
using Pos.Infrastructure.Persistence;
using Xunit;

namespace Pos.Tests;

public sealed class CajaTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public CajaTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Abrir_dos_sesiones_misma_caja_retorna_409()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var cajaId = await CrearCajaAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(PermissionCodes.CajaMovimiento);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var request = new CajaSesionAbrirDto(cajaId, 100m);

        var first = await client.PostAsJsonAsync("/api/v1/caja/sesiones/abrir", request);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await client.PostAsJsonAsync("/api/v1/caja/sesiones/abrir", request);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Registrar_movimiento_crea_audit_log()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var cajaId = await CrearCajaAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(PermissionCodes.CajaMovimiento);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var abrirResponse = await client.PostAsJsonAsync(
            "/api/v1/caja/sesiones/abrir",
            new CajaSesionAbrirDto(cajaId, 200m));

        Assert.Equal(HttpStatusCode.Created, abrirResponse.StatusCode);
        var sesion = await abrirResponse.Content.ReadFromJsonAsync<CajaSesionDto>();
        Assert.NotNull(sesion);

        var movimientoResponse = await client.PostAsJsonAsync(
            $"/api/v1/caja/sesiones/{sesion!.Id}/movimientos",
            new CajaMovimientoCreateDto("RETIRO", 50m, "Retiro test", "EFECTIVO"));

        Assert.Equal(HttpStatusCode.Created, movimientoResponse.StatusCode);
        var movimiento = await movimientoResponse.Content.ReadFromJsonAsync<CajaMovimientoDto>();
        Assert.NotNull(movimiento);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PosDbContext>();
        var audit = await db.AuditLogs.AsNoTracking()
            .FirstOrDefaultAsync(a => a.EntityName == "CajaMovimiento" && a.EntityId == movimiento!.Id.ToString());

        Assert.NotNull(audit);
        Assert.Equal(AuditAction.Adjust, audit!.Action);
        Assert.Equal(SeedData.TenantId, audit.TenantId);
        Assert.Equal(SeedData.SucursalId, audit.SucursalId);
    }

    [Fact]
    public async Task Cierre_con_diferencia_sin_motivo_retorna_400()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var cajaId = await CrearCajaAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(PermissionCodes.CajaMovimiento);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var abrirResponse = await client.PostAsJsonAsync(
            "/api/v1/caja/sesiones/abrir",
            new CajaSesionAbrirDto(cajaId, 100m));

        Assert.Equal(HttpStatusCode.Created, abrirResponse.StatusCode);
        var sesion = await abrirResponse.Content.ReadFromJsonAsync<CajaSesionDto>();
        Assert.NotNull(sesion);

        var cierreResponse = await client.PostAsJsonAsync(
            $"/api/v1/caja/sesiones/{sesion!.Id}/cerrar",
            new CajaCierreRequestDto(90m, Array.Empty<CajaCierreMedioDto>(), null));

        Assert.Equal(HttpStatusCode.BadRequest, cierreResponse.StatusCode);
    }

    [Fact]
    public async Task Cierre_correcto_retorna_ok()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var cajaId = await CrearCajaAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(PermissionCodes.CajaMovimiento);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var abrirResponse = await client.PostAsJsonAsync(
            "/api/v1/caja/sesiones/abrir",
            new CajaSesionAbrirDto(cajaId, 100m));

        Assert.Equal(HttpStatusCode.Created, abrirResponse.StatusCode);
        var sesion = await abrirResponse.Content.ReadFromJsonAsync<CajaSesionDto>();
        Assert.NotNull(sesion);

        var cierreResponse = await client.PostAsJsonAsync(
            $"/api/v1/caja/sesiones/{sesion!.Id}/cerrar",
            new CajaCierreRequestDto(100m, Array.Empty<CajaCierreMedioDto>(), null));

        Assert.Equal(HttpStatusCode.OK, cierreResponse.StatusCode);
        var payload = await cierreResponse.Content.ReadFromJsonAsync<CajaCierreResultDto>();
        Assert.NotNull(payload);
        Assert.Equal(0m, payload!.Diferencia);
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
