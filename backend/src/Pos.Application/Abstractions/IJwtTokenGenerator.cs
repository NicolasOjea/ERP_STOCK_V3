using Pos.Application.DTOs.Auth;

namespace Pos.Application.Abstractions;

public interface IJwtTokenGenerator
{
    JwtTokenResult GenerateToken(JwtTokenRequest request);
}
