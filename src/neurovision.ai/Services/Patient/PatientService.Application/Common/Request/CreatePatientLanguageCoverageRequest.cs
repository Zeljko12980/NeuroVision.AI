namespace PatientService.Application.Common.Request;

public sealed class CreatePatientLanguageCoverageRequest
{
    public Guid PatientId { get; set; }
    public string LanguageCode { get; set; } = null!;
}
