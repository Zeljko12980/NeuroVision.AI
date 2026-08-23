namespace DoctorService.UnitTests;

internal static class DoctorFactory
{
    public static readonly Guid DefaultId = Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1");
    public static readonly DateTime CreatedAt = new(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc);

    public static Doctor Create(
        Guid? id = null,
        string firstName = "Željko",
        string lastName = "Ikanović",
        string email = "ikanoviczeljko362@gmail.com",
        string phone = "+38761111222",
        string licenseNumber = "LIC-1001",
        string? licenseAuthorityCode = "KZK",
        string specializationCode = "NEURO",
        string statusCode = DoctorStatusCodes.Active,
        bool isAvailable = true,
        DateTime? createdAt = null)
    {
        return Doctor.Create(
            id ?? DefaultId,
            firstName,
            lastName,
            email,
            phone,
            licenseNumber,
            licenseAuthorityCode,
            specializationCode,
            statusCode,
            profilePictureUrl: "doctors/a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1.jpg",
            bio: "Neurologist.",
            healthInstitutionId: 1,
            institutionName: "Klinički centar Sarajevo",
            isAvailable,
            createdAt ?? CreatedAt);
    }
}
