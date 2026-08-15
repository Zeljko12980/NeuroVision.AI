namespace IdentityService.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        UserManager<AspIdentityUser> userManager,
        RoleManager<AspIdentityRole> roleManager,
        IdentitySeedOptions? seedOptions,
        ILogger logger)
    {
        await SeedRolesAsync(roleManager, logger);
        await SeedUserAsync(userManager, seedOptions?.SuperAdministrator, RoleNames.SuperAdministrator, logger);
        await SeedUserAsync(userManager, seedOptions?.Doctor, RoleNames.Doctor, logger);
    }

    private static async Task SeedRolesAsync(RoleManager<AspIdentityRole> roleManager, ILogger logger)
    {
        foreach (var definition in RoleNames.Definitions)
        {
            if (await roleManager.RoleExistsAsync(definition.Name))
            {
                logger.LogDebug("Seed role already exists. RoleName={RoleName}", definition.Name);
                continue;
            }

            var identityRole = AspIdentityRole.FromDomain(
                Role.Create(Guid.NewGuid(), definition.Name, definition.Description));

            var result = await roleManager.CreateAsync(identityRole);

            if (result.Succeeded)
            {
                logger.LogInformation("Seeded role. RoleName={RoleName}", definition.Name);
            }
            else
            {
                logger.LogError(
                    "Failed to seed role. RoleName={RoleName}, Errors={Errors}",
                    definition.Name,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    private static async Task SeedUserAsync(
        UserManager<AspIdentityUser> userManager,
        SeedUserOptions? options,
        string roleName,
        ILogger logger)
    {
        if (options is null || !options.IsConfigured)
        {
            logger.LogDebug("Skipping user seed. RoleName={RoleName}. Seed options are not configured.", roleName);
            return;
        }

        var existingUser = await userManager.FindByEmailAsync(options.Email);
        if (existingUser != null)
        {
            if (!await userManager.IsInRoleAsync(existingUser, roleName))
            {
                await userManager.AddToRoleAsync(existingUser, roleName);
                await userManager.AddClaimAsync(existingUser, new Claim("role", roleName));
                logger.LogInformation("Assigned seed role to existing user. Email={Email}, RoleName={RoleName}", options.Email, roleName);
            }

            return;
        }

        var user = new AspIdentityUser(
            Guid.NewGuid(),
            options.UserName,
            options.Email)
        {
            EmailConfirmed = true,
            TwoFactorEnabled = true
        };

        var result = await userManager.CreateAsync(user, options.Password);

        if (!result.Succeeded)
        {
            logger.LogError(
                "Failed to seed user. Email={Email}, RoleName={RoleName}, Errors={Errors}",
                options.Email,
                roleName,
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(user, roleName);
        await userManager.AddClaimAsync(user, new Claim("role", roleName));
        logger.LogInformation("Seeded user. Email={Email}, RoleName={RoleName}", options.Email, roleName);
    }
}
