using System.Collections.Generic;
using AuthService.Domain.Entities;
using AuthService.Application.Interfaces;
using AuthService.Application.Services;
using AuthService.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, IPasswordHashService passwordHashService)
    {
        // Verificar si ya existen roles
        if (!context.Roles.Any())
        {
            var roles = new List<Role>
            {
                new() {
                    Id = UuidGenerator.GenerateRoleId(),
                        Name = RoleConstants.ADMIN_ROLE
                },
                new() {
                    Id = UuidGenerator.GenerateRoleId(),
                        Name = RoleConstants.USER_ROLE
                }
            };
 
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }
 
        // Seed de un usuario administrador por defecto SOLO si no existen usuarios todavía
        if (!await context.Users.AnyAsync())
        {
            // Buscar rol admin existente
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == RoleConstants.ADMIN_ROLE);
            if (adminRole != null)
            {
                var userId = UuidGenerator.GenerateUserId();
                var profileId = UuidGenerator.GenerateUserId();
                var emailId = UuidGenerator.GenerateUserId();
                var userRoleId = UuidGenerator.GenerateUserId();
                var defaultAdminPassword = "Admin1234!";
 
                var adminUser = new User
                {
                    Id = userId,
                    Name = "Admin",
                    Surname = "User",
                    Username = "admin",
                    Email = "admin@ksports.local",
                    Password = passwordHashService.HashPassword(defaultAdminPassword),
                    Status = true,
                    UserProfile = new UserProfile
                    {
                        Id = profileId,
                        UserId = userId,
                    },
                    UserEmail = new UserEmail
                    {
                        Id = emailId,
                        UserId = userId,
                        EmailVerified = true,
                        EmailVerificationToken = null,
                        EmailVerificationTokenExpiry = null
                    },
                    UserRoles = new List<UserRole>
                    {
                        new UserRole
                        {
                            Id = userRoleId,
                            UserId = userId,
                            RoleId = adminRole.Id
                        }
                    }
                };
 
                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}