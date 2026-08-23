namespace DoctorService.Application.Common.Response;

public class DoctorLicenseHistoryResponse
{
    public Guid DoctorId { get; set; }
    public int SequenceNumber { get; set; }
    public string LicenseNumber { get; set; } = null!;
    public string? LicenseAuthorityCode { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
