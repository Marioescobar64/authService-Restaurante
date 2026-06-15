using AuthService.Domain.Entities;
using AuthService.Application.Services;
using AuthService.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var requiredRoles = new[]
        {
            RoleConstants.ADMIN_ROLE,
            RoleConstants.USER_ROLE,
            RoleConstants.MANAGER_ROLE,
            RoleConstants.CHEF_ROLE,
            RoleConstants.WAITER_ROLE
        };

        var existingRoleNames = await context.Roles.Select(r => r.Name).ToListAsync();
        var missingRoles = requiredRoles
            .Where(roleName => !existingRoleNames.Contains(roleName))
            .Select(roleName => new Role
            {
                Id = UuidGenerator.GenerateRoleId(),
                Name = roleName
            })
            .ToList();

        if (missingRoles.Count > 0)
        {
            await context.Roles.AddRangeAsync(missingRoles);
            await context.SaveChangesAsync();
        }
 
        // Seed de un usuario administrador por defecto SOLO si no existen usuarios todavía
        if (!await context.Users.AnyAsync())
        {
            // Buscar rol admin existente
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == RoleConstants.ADMIN_ROLE);
            if (adminRole != null)
            {
                var passwordHasher = new PasswordHashService();
 
                var userId = UuidGenerator.GenerateUserId();
                var profileId = UuidGenerator.GenerateUserId();
                var emailId = UuidGenerator.GenerateUserId();
                var userRoleId = UuidGenerator.GenerateUserId();
 
                var adminUser = new User
                {
                    Id = userId,
                    Name = "Admin",
                    Surname = "User",
                    Username = "admin",
                    Email = "admin@papaluigi.local",
                    Password = passwordHasher.HashPassword("papaluigi1!"),
                   //  Password = "12345678",
                    Status = true,
                    UserProfile = new UserProfile
                    {
                        Id = profileId,
                        UserId = userId,
                        //ProfilePicture = string.Empty,
                        //Phone = string.Empty
                    },
                    UserEmail = new UserEmail
                    {
                        Id = emailId,
                        UserId = userId,
                        EmailVerified = true,
                        EmailVerificationToken = null,
                        EmailVerificationTokenExpiry = null
                    },
                    UserRoles =
                    [
                        new UserRole
                        {
                            Id = userRoleId,
                            UserId = userId,
                            RoleId = adminRole.Id
                        }
                    ]
                };
 
                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}