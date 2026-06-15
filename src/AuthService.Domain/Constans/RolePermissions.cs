using AuthService.Domain.Entities;

namespace AuthService.Domain.Constants;

public static class RolePermissions
{
    public static string GetPrimaryRoleName(User? user)
    {
        if (user?.UserRoles == null || user.UserRoles.Count == 0)
        {
            return RoleConstants.USER_ROLE;
        }

        var roleName = user.UserRoles.FirstOrDefault()?.Role?.Name;
        return RoleConstants.NormalizeRoleName(roleName);
    }

    public static IReadOnlyList<string> GetModulesForRole(User? user)
    {
        return GetModulesForRole(GetPrimaryRoleName(user));
    }

    public static IReadOnlyList<string> GetModulesForRole(string? roleName)
    {
        var normalizedRole = RoleConstants.NormalizeRoleName(roleName);

        return normalizedRole switch
        {
            RoleConstants.MANAGER_ROLE => new[] { "orders", "events", "reservations", "staff", "stock", "tables", "cart" },
            RoleConstants.CHEF_ROLE => new[] { "orders" },
            RoleConstants.WAITER_ROLE => new[] { "orders", "tables", "cart" },
            RoleConstants.ADMIN_ROLE => new[] { "orders", "events", "reservations", "staff", "stock", "tables", "cart" },
            _ => Array.Empty<string>()
        };
    }
}
