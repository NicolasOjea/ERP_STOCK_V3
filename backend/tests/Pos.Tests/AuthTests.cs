using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Pos.Application.DTOs.Auth;
using Pos.Domain.Enums;
using Pos.Infrastructure.Persistence;
using Pos.WebApi.Auth;
using Xunit;

namespace Pos.Tests;

public sealed class AuthTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public AuthTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_ok_returns_token()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequestDto(
            "admin",
            "Admin123!",
            SeedData.TenantId,
            SeedData.SucursalId));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload!.Token));
        Assert.True(payload.ExpiresAt > DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task Protected_endpoint_without_token_returns_401()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/stock/adjust", new { productId = Guid.NewGuid(), quantity = 1 });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Protected_endpoint_with_token_without_permission_returns_403()
    {
        await _factory.EnsureDatabaseMigratedAsync();

        var client = _factory.CreateClient();
        var token = _factory.CreateTokenWithoutPermission();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/api/v1/stock/adjust", new { productId = Guid.NewGuid(), quantity = 1 });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}

public sealed class WebApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly JwtOptions _jwtOptions;

    public WebApiFactory()
    {
        _connectionString = Environment.GetEnvironmentVariable("TEST_DB_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=posdb;Username=pos;Password=pospass";

        _jwtOptions = new JwtOptions
        {
            Key = "test-secret-key-please-change-1234567890",
            Issuer = "pos",
            Audience = "pos",
            ExpiresMinutes = 30
        };
    }

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = _connectionString,
                ["Jwt:Key"] = _jwtOptions.Key,
                ["Jwt:Issuer"] = _jwtOptions.Issuer,
                ["Jwt:Audience"] = _jwtOptions.Audience,
                ["Jwt:ExpiresMinutes"] = _jwtOptions.ExpiresMinutes.ToString()
            };
            config.AddInMemoryCollection(settings);
        });
    }

    public async Task EnsureDatabaseMigratedAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PosDbContext>();
        await db.Database.MigrateAsync();
    }

    public string CreateTokenWithoutPermission()
    {
        return CreateTokenWithPermissions(PermissionCodes.VentaCrear);
    }

    public string CreateTokenWithPermissions(params string[] permissions)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new("tenant_id", SeedData.TenantId.ToString()),
            new("sucursal_id", SeedData.SucursalId.ToString()),
            new("user_id", SeedData.AdminUserId.ToString()),
            new("roles", "ADMIN")
        };

        foreach (var permission in permissions.Distinct())
        {
            claims.Add(new Claim("permissions", permission));
        }

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
