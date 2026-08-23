namespace PatientService.Application.Common.Request;

public sealed class CreatePatientAffiliationHistoryRequest
{
    public Guid PatientId { get; set; }
    public int? HealthInstitutionId { get; set; }
    public string InstitutionName { get; set; } = null!;
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
