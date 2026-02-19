namespace Pos.Application.DTOs.Auth;

public sealed record JwtTokenResult(string Token, DateTimeOffset ExpiresAt);
