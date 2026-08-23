namespace PatientService.Application.Common.Request;

public sealed class CreatePatientConditionCoverageRequest
{
    public Guid PatientId { get; set; }
    public string ConditionCode { get; set; } = null!;
    public int? DiagnosedYear { get; set; }
    public string? Note { get; set; }
}
