using Pos.Application.Abstractions;
using Pos.Application.DTOs.Auth;
using Pos.Domain.Exceptions;

namespace Pos.Application.UseCases.Auth;

public sealed class LoginService
{
    private readonly IAuthRepository _authRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IRequestContext _requestContext;
    private readonly IAuditLogService _auditLogService;

    public LoginService(
        IAuthRepository authRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator,
        IRequestContext requestContext,
        IAuditLogService auditLogService)
    {
        _authRepository = authRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _requestContext = requestContext;
        _auditLogService = auditLogService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["username"] = new[] { "El usuario es obligatorio." }
                });
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ValidationException(
                "Validacion fallida.",
                new Dictionary<string, string[]>
                {
                    ["password"] = new[] { "La contrasena es obligatoria." }
                });
        }

        var loginUser = await _authRepository.GetLoginUserAsync(
            request.Username.Trim(),
            request.TenantId,
            request.SucursalId,
            cancellationToken);

        if (loginUser is null || !loginUser.IsActive)
        {
            throw new UnauthorizedException("Credenciales invalidas.");
        }

        if (!_passwordHasher.Verify(request.Password, loginUser.PasswordHash))
        {
            throw new UnauthorizedException("Credenciales invalidas.");
        }

        var tokenResult = _tokenGenerator.GenerateToken(new JwtTokenRequest(
            loginUser.TenantId,
            loginUser.SucursalId,
            loginUser.UserId,
            loginUser.Roles,
            loginUser.Permissions));

        _requestContext.Set(loginUser.TenantId, loginUser.SucursalId, loginUser.UserId);
        await _auditLogService.LogAsync(
            "User",
            loginUser.UserId.ToString(),
            Pos.Domain.Enums.AuditAction.Login,
            null,
            System.Text.Json.JsonSerializer.Serialize(new { roles = loginUser.Roles, permissions = loginUser.Permissions }),
            null,
            cancellationToken);

        return new LoginResponseDto(tokenResult.Token, tokenResult.ExpiresAt);
    }
}
