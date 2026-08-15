using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IdentityContext context, UserManager<AspIdentityUser> userManager, RoleManager<AspIdentityRole> roleManager)
    {
        await SeedRolesAsync(roleManager);
        await SeedSuperAdministratorAsync(userManager, roleManager);
        await SeedDoctorAsync(userManager);
    }

    private static async Task SeedRolesAsync(RoleManager<AspIdentityRole> roleManager)
    {
        var roles = new[]
        {
            new { Name = "SuperAdministrator", Description = "System superadministrator with full access" },
            new { Name = "Administrator", Description = "Administrator with elevated privileges" },
            new { Name = "Doctor", Description = "Medical professional user" },
            new { Name = "Patient", Description = "Patient user" }
        };

        foreach (var role in roles)
        {
            var exists = await roleManager.RoleExistsAsync(role.Name);
            if (!exists)
            {
                var identityRole = new AspIdentityRole(
                    Guid.NewGuid(),
                    role.Name,
                    role.Description
                );
                await roleManager.CreateAsync(identityRole);
            }
        }
    }

    private static async Task SeedSuperAdministratorAsync(UserManager<AspIdentityUser> userManager, RoleManager<AspIdentityRole> roleManager)
    {
        const string superAdminEmail = "ikanoviczeljko095@gmail.com";
        const string superAdminUserName = "superadmin";
        const string superAdminPassword = "Zeljko123!";

        var existingUser = await userManager.FindByEmailAsync(superAdminEmail);
        if (existingUser != null)
        {
            return; 
        }

        var superAdmin = new AspIdentityUser(
            Guid.NewGuid(),
            superAdminUserName,
            superAdminEmail
        )
        {
            EmailConfirmed = true,
            TwoFactorEnabled = true
        };

        var result = await userManager.CreateAsync(superAdmin, superAdminPassword);

        if (result.Succeeded)
        {
            // Add SuperAdministrator role
            await userManager.AddToRoleAsync(superAdmin, "SuperAdministrator");

            // Add claim for role
            await userManager.AddClaimAsync(superAdmin, new System.Security.Claims.Claim("role", "SuperAdministrator"));
        }
    }

    private static async Task SeedDoctorAsync(UserManager<AspIdentityUser> userManager)
    {
        const string doctorEmail = "ikanoviczeljko362@gmail.com";
        const string doctorUserName = "doctor.zeljko";
        const string doctorPassword = "Zeljko123!";

        var existingUser = await userManager.FindByEmailAsync(doctorEmail);
        if (existingUser != null)
        {
            if (!await userManager.IsInRoleAsync(existingUser, "Doctor"))
            {
                await userManager.AddToRoleAsync(existingUser, "Doctor");
                await userManager.AddClaimAsync(existingUser, new System.Security.Claims.Claim("role", "Doctor"));
            }

            return;
        }

        var doctor = new AspIdentityUser(
            Guid.NewGuid(),
            doctorUserName,
            doctorEmail
        )
        {
            EmailConfirmed = true,
            TwoFactorEnabled = true
        };

        var result = await userManager.CreateAsync(doctor, doctorPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(doctor, "Doctor");
            await userManager.AddClaimAsync(doctor, new System.Security.Claims.Claim("role", "Doctor"));
        }
    }
}
