namespace Pos.Application.DTOs.Auth;

public sealed record LoginResponseDto(string Token, DateTimeOffset ExpiresAt);
