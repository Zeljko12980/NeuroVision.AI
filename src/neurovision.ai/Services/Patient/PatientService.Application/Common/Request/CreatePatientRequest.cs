using Microsoft.AspNetCore.Http;

namespace PatientService.Application.Common.Request;

public class CreatePatientRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public string? BloodType { get; set; }
    public string? NationalId { get; set; }
    public string Languages { get; set; } = null!;
    public string? Allergies { get; set; }
    public string? Conditions { get; set; }
    public string? Notes { get; set; }
    public string? Hospital { get; set; }
    public int? HealthInstitutionId { get; set; }
    public Guid? AssignedDoctorId { get; set; }
    public string? InsurancePayerCode { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public string? AddressLine { get; set; }
    public int? SettlementId { get; set; }
    public int? MunicipalityId { get; set; }
    public int? CountryId { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WeightKg { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyRelationshipCode { get; set; }
    public IFormFile? Picture { get; set; }
    public bool AutoActivate { get; set; }
}
