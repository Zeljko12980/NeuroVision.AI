namespace IdentityService.Domain.Constants;

public static class RoleNames
{
    public const string SuperAdministrator = "SuperAdministrator";
    public const string Administrator = "Administrator";
    public const string Doctor = "Doctor";
    public const string Patient = "Patient";

    public static readonly IReadOnlyList<RoleDefinition> Definitions =
    [
        new(SuperAdministrator, "System superadministrator with full access"),
        new(Administrator, "Administrator with elevated privileges"),
        new(Doctor, "Medical professional user"),
        new(Patient, "Patient user")
    ];
}

public sealed record RoleDefinition(string Name, string Description);
