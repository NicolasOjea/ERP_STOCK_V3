using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Auth;

namespace Pos.WebApi.Auth;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;

    public JwtTokenGenerator(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public JwtTokenResult GenerateToken(JwtTokenRequest request)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new("tenant_id", request.TenantId.ToString()),
            new("sucursal_id", request.SucursalId.ToString()),
            new("user_id", request.UserId.ToString())
        };

        foreach (var role in request.Roles.Distinct())
        {
            claims.Add(new Claim("roles", role));
        }

        foreach (var permission in request.Permissions.Distinct())
        {
            claims.Add(new Claim("permissions", permission));
        }

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.ExpiresMinutes);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return new JwtTokenResult(tokenValue, expiresAt);
    }
}
