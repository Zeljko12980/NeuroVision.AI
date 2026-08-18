namespace IdentityService.Infrastructure.Configuration;

public class IdentitySeedOptions
{
    public const string SectionName = "IdentitySeed";

    public SeedUserOptions? SuperAdministrator { get; set; }
    public SeedUserOptions? Doctor { get; set; }
}

public class SeedUserOptions
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Email)
        && !string.IsNullOrWhiteSpace(UserName)
        && !string.IsNullOrWhiteSpace(Password);
}
