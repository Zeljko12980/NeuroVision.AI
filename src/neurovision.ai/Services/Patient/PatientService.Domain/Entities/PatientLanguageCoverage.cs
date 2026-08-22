namespace PatientService.Domain.Entities;

public class PatientLanguageCoverage
{
    public Guid PatientId { get; private set; }
    public string LanguageCode { get; private set; } = null!;

    public Patient Patient { get; private set; } = null!;
    public Language Language { get; private set; } = null!;

    private PatientLanguageCoverage()
    {
    }

    public static PatientLanguageCoverage Create(Guid patientId, string languageCode)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient id is required.", nameof(patientId));

        return new PatientLanguageCoverage
        {
            PatientId = patientId,
            LanguageCode = Guard.Code(languageCode, nameof(languageCode))
        };
    }
}
