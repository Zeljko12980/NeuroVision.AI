namespace DoctorService.Domain.Entities;

public class DoctorAffiliationHistory
{
    public Guid DoctorId { get; private set; }
    public int SequenceNumber { get; private set; }
    public int? HealthInstitutionId { get; private set; }
    public string InstitutionName { get; private set; } = null!;
    public DateTime From { get; private set; }
    public DateTime? To { get; private set; }

    public Doctor Doctor { get; private set; } = null!;

    private DoctorAffiliationHistory()
    {
    }

    public static DoctorAffiliationHistory Create(
        Guid doctorId,
        int sequenceNumber,
        string institutionName,
        int? healthInstitutionId,
        DateTime from,
        DateTime? to = null)
    {
        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor id is required.", nameof(doctorId));

        if (sequenceNumber <= 0)
            throw new ArgumentException("Sequence number must be greater than zero.", nameof(sequenceNumber));

        if (healthInstitutionId is <= 0)
            throw new ArgumentException("Health institution id must be greater than zero.", nameof(healthInstitutionId));

        DateRange.EnsureValid(from, to);

        return new DoctorAffiliationHistory
        {
            DoctorId = doctorId,
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
