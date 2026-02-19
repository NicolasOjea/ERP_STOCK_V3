using System.Security.Claims;
using Pos.Application.Abstractions;

namespace Pos.WebApi.Middleware;

public sealed class RequestContextMiddleware
{
    private readonly RequestDelegate _next;

    public RequestContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IRequestContext requestContext)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var tenantId = GetGuidClaim(context.User, "tenant_id");
            var sucursalId = GetGuidClaim(context.User, "sucursal_id");
            var userId = GetGuidClaim(context.User, "user_id");

            if (tenantId.HasValue && sucursalId.HasValue && userId.HasValue)
            {
                requestContext.Set(tenantId.Value, sucursalId.Value, userId.Value);
            }
        }

        await _next(context);
    }

    private static Guid? GetGuidClaim(ClaimsPrincipal user, string type)
    {
        var value = user.FindFirstValue(type);
        return Guid.TryParse(value, out var parsed) ? parsed : null;
    }
}
