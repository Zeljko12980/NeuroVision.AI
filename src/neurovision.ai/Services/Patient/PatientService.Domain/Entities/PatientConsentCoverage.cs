namespace PatientService.Domain.Entities;

public class PatientConsentCoverage
{
    public Guid PatientId { get; private set; }
    public string ConsentTypeCode { get; private set; } = null!;
    public DateTime From { get; private set; }
    public DateTime? To { get; private set; }

    public Patient Patient { get; private set; } = null!;
    public ConsentType ConsentType { get; private set; } = null!;

    private PatientConsentCoverage()
    {
    }

    public static PatientConsentCoverage Create(
        Guid patientId,
        string consentTypeCode,
        DateTime from,
        DateTime? to = null)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient id is required.", nameof(patientId));

        DateRange.EnsureValid(from, to);

        return new PatientConsentCoverage
        {
            PatientId = patientId,
            ConsentTypeCode = Guard.Code(consentTypeCode, nameof(consentTypeCode)),
            From = from,
            To = to
        };
    }

    public void Revoke(DateTime at)
    {
        DateRange.EnsureValid(From, at);
        To = at;
    }

    public void Reopen(DateTime from)
    {
        From = from;
        To = null;
    }
}
