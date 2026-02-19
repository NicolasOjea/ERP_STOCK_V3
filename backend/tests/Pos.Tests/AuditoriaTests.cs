using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Pos.Application.DTOs.Auditoria;
using Pos.Domain.Entities;
using Pos.Domain.Enums;
using Pos.Infrastructure.Persistence;
using Xunit;

namespace Pos.Tests;

public sealed class AuditoriaTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public AuditoriaTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task No_devuelve_logs_de_otro_tenant()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var entidad = $"TEST_AUDIT_{Guid.NewGuid():N}";
        var now = DateTimeOffset.UtcNow;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PosDbContext>();

            var otherTenantId = Guid.NewGuid();
            var otherSucursalId = Guid.NewGuid();

            db.Tenants.Add(new Tenant(otherTenantId, "Otro", now));
            db.Sucursales.Add(new Sucursal(otherSucursalId, otherTenantId, "Sucursal Otro", "OTRO", now));

            db.AuditLogs.Add(new AuditLog(
                Guid.NewGuid(),
                SeedData.TenantId,
                SeedData.SucursalId,
                SeedData.AdminUserId,
                AuditAction.Create,
                entidad,
                "1",
                now,
                null,
                null,
                null));

            db.AuditLogs.Add(new AuditLog(
                Guid.NewGuid(),
                otherTenantId,
                otherSucursalId,
                null,
                AuditAction.Create,
                entidad,
                "2",
                now,
                null,
                null,
                null));

            await db.SaveChangesAsync();
        }

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(PermissionCodes.ReportesVer);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"/api/v1/auditoria?entidad={entidad}&page=1&size=50");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<AuditLogQueryResultDto>();
        Assert.NotNull(result);
        Assert.All(result!.Items, item => Assert.Equal(entidad, item.Entidad));
        Assert.Single(result.Items);
    }
}
