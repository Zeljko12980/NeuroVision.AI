namespace DoctorService.Application.Common.Request;

public sealed class CreateDoctorSpecializationCoverageRequest
{
    public Guid DoctorId { get; set; }
    public string SpecializationCode { get; set; } = null!;
    public bool IsPrimary { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
