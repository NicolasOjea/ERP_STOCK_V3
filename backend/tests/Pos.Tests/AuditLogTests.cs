using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pos.Application.DTOs.Users;
using Pos.Domain.Enums;
using Pos.Infrastructure.Persistence;
using Xunit;

namespace Pos.Tests;

public sealed class AuditLogTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public AuditLogTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Updating_roles_creates_audit_log()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithPermissions(PermissionCodes.UsuarioAdmin);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.PutAsJsonAsync(
            $"/api/v1/users/{SeedData.AdminUserId}/roles",
            new UpdateUserRolesRequestDto(new[] { "ADMIN" }));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PosDbContext>();
        var audit = await db.AuditLogs.AsNoTracking()
            .Where(a => a.EntityName == "UserRole" && a.EntityId == SeedData.AdminUserId.ToString())
            .OrderByDescending(a => a.OccurredAt)
            .FirstOrDefaultAsync();

        Assert.NotNull(audit);
        Assert.Equal(AuditAction.RoleChange, audit!.Action);
        Assert.Equal(SeedData.TenantId, audit.TenantId);
        Assert.Equal(SeedData.SucursalId, audit.SucursalId);
    }
}
