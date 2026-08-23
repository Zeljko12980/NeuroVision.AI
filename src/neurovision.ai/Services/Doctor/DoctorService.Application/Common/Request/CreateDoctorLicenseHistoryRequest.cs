namespace DoctorService.Application.Common.Request;

public sealed class CreateDoctorLicenseHistoryRequest
{
    public Guid DoctorId { get; set; }
    public string LicenseNumber { get; set; } = null!;
    public string? LicenseAuthorityCode { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
