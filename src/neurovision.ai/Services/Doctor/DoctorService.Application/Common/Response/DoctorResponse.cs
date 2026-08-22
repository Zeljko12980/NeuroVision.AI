namespace DoctorService.Application.Common.Response;

public class DoctorResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public string? LicenseAuthorityCode { get; set; }
    public string CurrentSpecializationCode { get; set; } = null!;
    public string CurrentStatusCode { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public int? CurrentHealthInstitutionId { get; set; }
    public string? CurrentInstitutionName { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime LastActive { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public DateTime CreatedAt { get; set; }
}
