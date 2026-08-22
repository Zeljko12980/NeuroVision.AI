using Microsoft.AspNetCore.Http;

namespace DoctorService.Application.Common.Request;

public class CreateDoctorRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public string? LicenseAuthorityCode { get; set; }
    public string Specialization { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Languages { get; set; } = null!;
    public IFormFile? Picture { get; set; }
    public string? Bio { get; set; }
    public string? Degrees { get; set; }
    public string? Hospital { get; set; }
    public int? HealthInstitutionId { get; set; }
    public bool IsAvailable { get; set; }
    public bool AutoActivate { get; set; }
}
