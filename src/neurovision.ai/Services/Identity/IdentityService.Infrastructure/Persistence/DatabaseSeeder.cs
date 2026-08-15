namespace IdentityService.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        UserManager<AspIdentityUser> userManager,
        RoleManager<AspIdentityRole> roleManager,
        IdentitySeedOptions? seedOptions)
    {
        await SeedRolesAsync(roleManager);
        await SeedUserAsync(userManager, seedOptions?.SuperAdministrator, RoleNames.SuperAdministrator);
        await SeedUserAsync(userManager, seedOptions?.Doctor, RoleNames.Doctor);
    }

    private static async Task SeedRolesAsync(RoleManager<AspIdentityRole> roleManager)
    {
        foreach (var definition in RoleNames.Definitions)
        {
            if (await roleManager.RoleExistsAsync(definition.Name))
                continue;

            var identityRole = AspIdentityRole.FromDomain(
                Role.Create(Guid.NewGuid(), definition.Name, definition.Description));

            await roleManager.CreateAsync(identityRole);
        }
    }

    private static async Task SeedUserAsync(
        UserManager<AspIdentityUser> userManager,
        SeedUserOptions? options,
        string roleName)
    {
        if (options is null || !options.IsConfigured)
            return;

        var existingUser = await userManager.FindByEmailAsync(options.Email);
        if (existingUser != null)
        {
            if (!await userManager.IsInRoleAsync(existingUser, roleName))
            {
                await userManager.AddToRoleAsync(existingUser, roleName);
                await userManager.AddClaimAsync(existingUser, new Claim("role", roleName));
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

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, roleName);
            await userManager.AddClaimAsync(user, new Claim("role", roleName));
        }
    }
}
