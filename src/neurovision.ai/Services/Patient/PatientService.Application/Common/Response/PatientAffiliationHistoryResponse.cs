namespace PatientService.Application.Common.Response;

public class PatientAffiliationHistoryResponse
{
    public Guid PatientId { get; set; }
    public int SequenceNumber { get; set; }
    public int? HealthInstitutionId { get; set; }
    public string InstitutionName { get; set; } = null!;
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
