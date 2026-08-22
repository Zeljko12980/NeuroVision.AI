namespace PatientService.Domain.Entities;

public class PatientStatusHistory
{
    public Guid PatientId { get; private set; }
    public int SequenceNumber { get; private set; }
    public string StatusCode { get; private set; } = null!;
    public DateTime From { get; private set; }
    public DateTime? To { get; private set; }

    public Patient Patient { get; private set; } = null!;
    public PatientStatus Status { get; private set; } = null!;

    private PatientStatusHistory()
    {
    }

    public static PatientStatusHistory Create(
        Guid patientId,
        int sequenceNumber,
        string statusCode,
        DateTime from,
        DateTime? to = null)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient id is required.", nameof(patientId));

        if (sequenceNumber <= 0)
            throw new ArgumentException("Sequence number must be greater than zero.", nameof(sequenceNumber));

        DateRange.EnsureValid(from, to);

        return new PatientStatusHistory
        {
            PatientId = patientId,
            SequenceNumber = sequenceNumber,
            StatusCode = Guard.Code(statusCode, nameof(statusCode)),
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
