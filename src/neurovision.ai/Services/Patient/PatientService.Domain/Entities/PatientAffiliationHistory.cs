namespace PatientService.Domain.Entities;

public class PatientAffiliationHistory
{
    public Guid PatientId { get; private set; }
    public int SequenceNumber { get; private set; }
    public int? HealthInstitutionId { get; private set; }
    public string InstitutionName { get; private set; } = null!;
    public DateTime From { get; private set; }
    public DateTime? To { get; private set; }

    public Patient Patient { get; private set; } = null!;

    private PatientAffiliationHistory()
    {
    }

    public static PatientAffiliationHistory Create(
        Guid patientId,
        int sequenceNumber,
        string institutionName,
        int? healthInstitutionId,
        DateTime from,
        DateTime? to = null)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient id is required.", nameof(patientId));

        if (sequenceNumber <= 0)
            throw new ArgumentException("Sequence number must be greater than zero.", nameof(sequenceNumber));

        if (healthInstitutionId is <= 0)
            throw new ArgumentException("Health institution id must be greater than zero.", nameof(healthInstitutionId));

        DateRange.EnsureValid(from, to);

        return new PatientAffiliationHistory
        {
            PatientId = patientId,
            SequenceNumber = sequenceNumber,
            HealthInstitutionId = healthInstitutionId,
            InstitutionName = Guard.NotEmpty(institutionName, nameof(institutionName)),
            From = from,
            To = to
        };
    }

    public void Close(DateTime to)
    {
        DateRange.EnsureValid(From, to);
        To = to;
    }
}
