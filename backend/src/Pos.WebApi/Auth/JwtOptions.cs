namespace Pos.WebApi.Auth;

public sealed class JwtOptions
{
    public string Key { get; init; } = "dev-secret-key-please-change-1234567890";
    public string Issuer { get; init; } = "pos";
    public string Audience { get; init; } = "pos";
    public int ExpiresMinutes { get; init; } = 60;
}
