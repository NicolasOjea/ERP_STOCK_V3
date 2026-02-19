using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.Application.DTOs.Users;
using Pos.Application.UseCases.Users;

namespace Pos.WebApi.Controllers;

[ApiController]
[Route("api/v1/users")]
public sealed class UsersController : ControllerBase
{
    private readonly UpdateUserRolesService _updateUserRolesService;

    public UsersController(UpdateUserRolesService updateUserRolesService)
    {
        _updateUserRolesService = updateUserRolesService;
    }

    [HttpPut("{userId:guid}/roles")]
    [Authorize(Policy = "PERM_USUARIO_ADMIN")]
    public async Task<IActionResult> UpdateRoles(Guid userId, [FromBody] UpdateUserRolesRequestDto request, CancellationToken cancellationToken)
    {
        await _updateUserRolesService.UpdateRolesAsync(userId, request, cancellationToken);
        return NoContent();
    }
}
