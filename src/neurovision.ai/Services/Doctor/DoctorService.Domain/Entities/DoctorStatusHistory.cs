namespace DoctorService.Domain.Entities;

public class DoctorStatusHistory
{
    public Guid DoctorId { get; private set; }
    public int SequenceNumber { get; private set; }
    public string StatusCode { get; private set; } = null!;
    public DateTime From { get; private set; }
    public DateTime? To { get; private set; }

    public Doctor Doctor { get; private set; } = null!;
    public DoctorStatus Status { get; private set; } = null!;

    private DoctorStatusHistory()
    {
    }

    public static DoctorStatusHistory Create(
        Guid doctorId,
        int sequenceNumber,
        string statusCode,
        DateTime from,
        DateTime? to = null)
    {
        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor id is required.", nameof(doctorId));

        if (sequenceNumber <= 0)
            throw new ArgumentException("Sequence number must be greater than zero.", nameof(sequenceNumber));

        DateRange.EnsureValid(from, to);

        return new DoctorStatusHistory
        {
            DoctorId = doctorId,
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
