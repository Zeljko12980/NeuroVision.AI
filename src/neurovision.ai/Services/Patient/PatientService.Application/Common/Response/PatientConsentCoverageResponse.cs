namespace PatientService.Application.Common.Response;

public class PatientConsentCoverageResponse
{
    public Guid PatientId { get; set; }
    public string ConsentTypeCode { get; set; } = null!;
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
