namespace PatientService.Application.Common.Response;

public class PatientResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string GenderCode { get; set; } = null!;
    public string? BloodTypeCode { get; set; }
    public string? NationalId { get; set; }
    public string CurrentStatusCode { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; }
    public string? Notes { get; set; }
    public int? CurrentHealthInstitutionId { get; set; }
    public string? CurrentInstitutionName { get; set; }
    public Guid? AssignedDoctorId { get; set; }
    public string? CurrentInsurancePayerCode { get; set; }
    public string? CurrentInsurancePolicyNumber { get; set; }
    public string? AddressLine { get; set; }
    public int? SettlementId { get; set; }
    public int? MunicipalityId { get; set; }
    public int? CountryId { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WeightKg { get; set; }
    public DateTime LastActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
