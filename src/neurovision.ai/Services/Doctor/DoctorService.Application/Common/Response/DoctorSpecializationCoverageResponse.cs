namespace DoctorService.Application.Common.Response;

public class DoctorSpecializationCoverageResponse
{
    public Guid DoctorId { get; set; }
    public string SpecializationCode { get; set; } = null!;
    public bool IsPrimary { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
