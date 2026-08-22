namespace DoctorService.Domain.Entities;

public class DoctorLanguageCoverage
{
    public Guid DoctorId { get; private set; }
    public string LanguageCode { get; private set; } = null!;

    public Doctor Doctor { get; private set; } = null!;
    public Language Language { get; private set; } = null!;

    private DoctorLanguageCoverage()
    {
    }

    public static DoctorLanguageCoverage Create(Guid doctorId, string languageCode)
    {
        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor id is required.", nameof(doctorId));

        return new DoctorLanguageCoverage
        {
            DoctorId = doctorId,
            LanguageCode = Guard.Code(languageCode, nameof(languageCode))
        };
    }
}
