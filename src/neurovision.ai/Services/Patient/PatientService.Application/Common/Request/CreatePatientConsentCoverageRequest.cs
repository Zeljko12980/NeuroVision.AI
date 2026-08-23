namespace PatientService.Application.Common.Request;

public sealed class CreatePatientConsentCoverageRequest
{
    public Guid PatientId { get; set; }
    public string ConsentTypeCode { get; set; } = null!;
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
