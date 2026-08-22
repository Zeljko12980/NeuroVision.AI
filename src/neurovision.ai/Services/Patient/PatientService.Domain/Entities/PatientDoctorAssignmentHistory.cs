namespace PatientService.Domain.Entities;

public class PatientDoctorAssignmentHistory
{
    public Guid PatientId { get; private set; }
    public int SequenceNumber { get; private set; }
    public Guid DoctorId { get; private set; }
    public DateTime From { get; private set; }
    public DateTime? To { get; private set; }

    public Patient Patient { get; private set; } = null!;

    private PatientDoctorAssignmentHistory()
    {
    }

    public static PatientDoctorAssignmentHistory Create(
        Guid patientId,
        int sequenceNumber,
        Guid doctorId,
        DateTime from,
        DateTime? to = null)
    {
        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient id is required.", nameof(patientId));

        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor id is required.", nameof(doctorId));

        if (sequenceNumber <= 0)
            throw new ArgumentException("Sequence number must be greater than zero.", nameof(sequenceNumber));

        DateRange.EnsureValid(from, to);

        return new PatientDoctorAssignmentHistory
        {
            PatientId = patientId,
            SequenceNumber = sequenceNumber,
            DoctorId = doctorId,
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
