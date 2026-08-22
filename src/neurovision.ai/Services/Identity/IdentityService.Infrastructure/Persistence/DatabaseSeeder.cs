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

        var user = await FindSeedUserAsync(userManager, options);

        if (user is null)
        {
            user = new AspIdentityUser(
                options.Id ?? Guid.NewGuid(),
                options.UserName,
                options.Email)
            {
                EmailConfirmed = true,
                TwoFactorEnabled = true
            };

            var createResult = await userManager.CreateAsync(user, options.Password);
            if (!createResult.Succeeded)
            {
                logger.LogError(
                    "Failed to seed user. Email={Email}, RoleName={RoleName}, Errors={Errors}",
                    options.Email,
                    roleName,
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
                return;
            }

            await EnsureRoleAsync(userManager, user, roleName, logger);
            logger.LogInformation("Seeded user. Email={Email}, RoleName={RoleName}", options.Email, roleName);
            return;
        }

        await SyncSeedUserAsync(userManager, user, options, logger);
        await EnsureSeedPasswordAsync(userManager, user, options.Password, logger);
        await EnsureRoleAsync(userManager, user, roleName, logger);
    }

    private static async Task<AspIdentityUser?> FindSeedUserAsync(
        UserManager<AspIdentityUser> userManager,
        SeedUserOptions options)
    {
        var user = await userManager.FindByEmailAsync(options.Email);
        if (user is not null)
            return user;

        if (options.Id is Guid seedId)
        {
            user = await userManager.FindByIdAsync(seedId.ToString());
            if (user is not null)
                return user;
        }

        if (!string.IsNullOrWhiteSpace(options.UserName))
            return await userManager.FindByNameAsync(options.UserName);

        return null;
    }

    private static async Task SyncSeedUserAsync(
        UserManager<AspIdentityUser> userManager,
        AspIdentityUser user,
        SeedUserOptions options,
        ILogger logger)
    {
        if (!string.Equals(user.Email, options.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailResult = await userManager.SetEmailAsync(user, options.Email);
            if (!emailResult.Succeeded)
            {
                logger.LogError(
                    "Failed to update seed email. UserId={UserId}, Email={Email}, Errors={Errors}",
                    user.Id,
                    options.Email,
                    string.Join(", ", emailResult.Errors.Select(e => e.Description)));
            }
            else
            {
                logger.LogInformation(
                    "Updated seed email. UserId={UserId}, Email={Email}",
                    user.Id,
                    options.Email);
            }
        }

        if (!string.Equals(user.UserName, options.UserName, StringComparison.OrdinalIgnoreCase))
        {
            var nameResult = await userManager.SetUserNameAsync(user, options.UserName);
            if (!nameResult.Succeeded)
            {
                logger.LogError(
                    "Failed to update seed username. UserId={UserId}, UserName={UserName}, Errors={Errors}",
                    user.Id,
                    options.UserName,
                    string.Join(", ", nameResult.Errors.Select(e => e.Description)));
            }
        }

        var changed = false;
        if (!user.EmailConfirmed)
        {
            user.EmailConfirmed = true;
            changed = true;
        }

        if (!user.TwoFactorEnabled)
        {
            user.TwoFactorEnabled = true;
            changed = true;
        }

        if (changed)
            await userManager.UpdateAsync(user);

        await userManager.ResetAccessFailedCountAsync(user);
        if (await userManager.IsLockedOutAsync(user))
            await userManager.SetLockoutEndDateAsync(user, null);
    }

    private static async Task EnsureSeedPasswordAsync(
        UserManager<AspIdentityUser> userManager,
        AspIdentityUser user,
        string password,
        ILogger logger)
    {
        var hasPassword = await userManager.HasPasswordAsync(user);
        var passwordMatches = hasPassword && await userManager.CheckPasswordAsync(user, password);
        if (passwordMatches)
            return;

        if (hasPassword)
            await userManager.RemovePasswordAsync(user);

        var result = await userManager.AddPasswordAsync(user, password);
        if (!result.Succeeded)
        {
            logger.LogError(
                "Failed to set seed password. Email={Email}, Errors={Errors}",
                user.Email,
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        logger.LogInformation("Reset seed user password. Email={Email}", user.Email);
    }

    private static async Task EnsureRoleAsync(
        UserManager<AspIdentityUser> userManager,
        AspIdentityUser user,
        string roleName,
        ILogger logger)
    {
        if (await userManager.IsInRoleAsync(user, roleName))
            return;

        await userManager.AddToRoleAsync(user, roleName);
        await userManager.AddClaimAsync(user, new Claim("role", roleName));
        logger.LogInformation(
            "Assigned seed role to existing user. Email={Email}, RoleName={RoleName}",
            user.Email,
            roleName);
    }
}
