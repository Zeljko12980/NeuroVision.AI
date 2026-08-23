namespace BuildingBlocks.Auth;

public static class AuthPolicies
{
    public const string SuperAdmin = "SuperAdminPolicy";
    public const string Doctor = "DoctorPolicy";
    public const string Patient = "PatientPolicy";
    public const string Staff = "StaffPolicy";
}

public static class AuthRoles
{
    public const string SuperAdministrator = "SuperAdministrator";
    public const string Doctor = "Doctor";
    public const string Patient = "Patient";
}
