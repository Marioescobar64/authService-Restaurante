using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UserManagementController(IUserManagementService userManagementService) : ControllerBase
{
    [HttpPost("role")]
    public async Task<ActionResult<UserResponseDto>> UpdateUserRole([FromBody] UpdateUserRoleDto request)
    {
        var updatedUser = await userManagementService.UpdateUserRoleAsync(request.UserId, request.RoleName);
        return Ok(updatedUser);
    }

    [HttpGet("roles/{userId}")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetUserRoles(string userId)
    {
        var roles = await userManagementService.GetUserRolesAsync(userId);
        return Ok(roles);
    }

    [HttpGet("role/{roleName}")]
    public async Task<ActionResult<IReadOnlyList<UserResponseDto>>> GetUsersByRole(string roleName)
    {
        var users = await userManagementService.GetUsersByRoleAsync(roleName);
        return Ok(users);
    }
}
