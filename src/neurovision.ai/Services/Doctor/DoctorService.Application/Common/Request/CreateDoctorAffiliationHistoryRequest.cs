namespace DoctorService.Application.Common.Request;

public sealed class CreateDoctorAffiliationHistoryRequest
{
    public Guid DoctorId { get; set; }
    public int? HealthInstitutionId { get; set; }
    public string InstitutionName { get; set; } = null!;
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
