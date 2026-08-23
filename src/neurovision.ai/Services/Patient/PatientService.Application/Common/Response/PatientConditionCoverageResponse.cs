namespace PatientService.Application.Common.Response;

public class PatientConditionCoverageResponse
{
    public Guid PatientId { get; set; }
    public string ConditionCode { get; set; } = null!;
    public int? DiagnosedYear { get; set; }
    public string? Note { get; set; }
}
