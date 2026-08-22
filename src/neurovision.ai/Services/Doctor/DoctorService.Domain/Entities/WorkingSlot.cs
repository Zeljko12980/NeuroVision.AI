namespace DoctorService.Domain.Entities;

public class WorkingSlot
{
    public Guid DoctorId { get; private set; }
    public int DayOfWeek { get; private set; }
    public int SequenceNumber { get; private set; }
    public TimeSpan Start { get; private set; }
    public TimeSpan End { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }

    public Doctor Doctor { get; private set; } = null!;

    private WorkingSlot()
    {
    }

    public static WorkingSlot Create(
        Guid doctorId,
        int dayOfWeek,
        int sequenceNumber,
        TimeSpan start,
        TimeSpan end,
        DateTime validFrom,
        DateTime? validTo = null)
    {
        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor id is required.", nameof(doctorId));

        if (dayOfWeek is < 0 or > 6)
            throw new ArgumentException("Day of week must be between 0 and 6.", nameof(dayOfWeek));

        if (sequenceNumber <= 0)
            throw new ArgumentException("Sequence number must be greater than zero.", nameof(sequenceNumber));

        if (end <= start)
            throw new ArgumentException("Slot end must be after start.", nameof(end));

        DateRange.EnsureValid(validFrom, validTo);

        return new WorkingSlot
        {
            DoctorId = doctorId,
            DayOfWeek = dayOfWeek,
            SequenceNumber = sequenceNumber,
            Start = start,
            End = end,
            ValidFrom = validFrom,
            ValidTo = validTo
        };
    }

    public void Close(DateTime validTo)
    {
        DateRange.EnsureValid(ValidFrom, validTo);
        ValidTo = validTo;
    }
}
