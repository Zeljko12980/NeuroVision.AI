namespace PatientService.UnitTests;

internal static class PatientFactory
{
    public static readonly Guid DefaultId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid DoctorId = Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1");
    public static readonly DateTime CreatedAt = new(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc);

    public static Patient Create(
        Guid? id = null,
        string firstName = "Haris",
        string lastName = "Delić",
        string email = "armanigas78@gmail.com",
        string phone = "+38762222333",
        DateTime? dateOfBirth = null,
        string genderCode = GenderCodes.Male,
        string statusCode = PatientStatusCodes.Active,
        string? bloodTypeCode = "OP",
        Guid? assignedDoctorId = null,
        decimal? heightCm = 178,
        decimal? weightKg = 82,
        DateTime? createdAt = null)
    {
        return Patient.Create(
            id ?? DefaultId,
            firstName,
            lastName,
            email,
            phone,
            dateOfBirth ?? new DateTime(1975, 9, 3),
            genderCode,
            statusCode,
            bloodTypeCode,
            nationalId: "0309975123456",
            profilePictureUrl: "patients/22222222-2222-2222-2222-222222222222.jpg",
            notes: "Drug-resistant focal epilepsy.",
            healthInstitutionId: 1,
            institutionName: "Klinički centar Sarajevo",
            assignedDoctorId ?? DoctorId,
            addressLine: "Titova 8",
            settlementId: 1,
            municipalityId: 1,
            countryId: 1,
            heightCm,
            weightKg,
            createdAt ?? CreatedAt);
    }
}
