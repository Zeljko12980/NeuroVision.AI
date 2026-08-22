namespace DoctorService.Domain.Entities;

public class DoctorSpecializationCoverage
{
    public Guid DoctorId { get; private set; }
    public string SpecializationCode { get; private set; } = null!;
    public bool IsPrimary { get; private set; }
    public DateTime From { get; private set; }
    public DateTime? To { get; private set; }

    public Doctor Doctor { get; private set; } = null!;
    public Specialization Specialization { get; private set; } = null!;

    private DoctorSpecializationCoverage()
    {
    }

    public static DoctorSpecializationCoverage Create(
        Guid doctorId,
        string specializationCode,
        bool isPrimary,
        DateTime from,
        DateTime? to = null)
    {
        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor id is required.", nameof(doctorId));

        DateRange.EnsureValid(from, to);

        return new DoctorSpecializationCoverage
        {
            DoctorId = doctorId,
            SpecializationCode = Guard.Code(specializationCode, nameof(specializationCode)),
            IsPrimary = isPrimary,
            From = from,
            To = to
        };
    }

    public void Close(DateTime to)
    {
        DateRange.EnsureValid(From, to);
        To = to;
        IsPrimary = false;
    }

    public void Reopen(DateTime from)
    {
        From = from;
        To = null;
        IsPrimary = true;
    }
}
