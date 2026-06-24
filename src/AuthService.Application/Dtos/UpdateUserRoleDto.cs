namespace AuthService.Application.DTOs;

public class UpdateUserRoleDto
{
    // Single role per user; accept role name like "ADMIN_ROLE" or "USER_ROLE"
    public string UserId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}