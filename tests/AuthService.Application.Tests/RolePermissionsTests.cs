using AuthService.Domain.Constants;
using Xunit;

namespace AuthService.Application.Tests;

public class RolePermissionsTests
{
    [Theory]
    [InlineData(RoleConstants.MANAGER_ROLE, new[] { "orders", "events", "reservations", "staff", "stock", "tables", "cart" })]
    [InlineData(RoleConstants.CHEF_ROLE, new[] { "orders" })]
    [InlineData(RoleConstants.WAITER_ROLE, new[] { "orders", "tables", "cart" })]
    public void ShouldExposeExpectedModulesForEachRole(string roleName, string[] expectedModules)
    {
        var modules = RolePermissions.GetModulesForRole(roleName);

        Assert.Equal(expectedModules, modules);
    }
}
