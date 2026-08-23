namespace PatientService.Application.Common.Response;

public class PatientLanguageCoverageResponse
{
    public Guid PatientId { get; set; }
    public string LanguageCode { get; set; } = null!;
}
