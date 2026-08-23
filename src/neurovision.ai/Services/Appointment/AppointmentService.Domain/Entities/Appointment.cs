namespace AppointmentService.Domain.Entities;

public class Appointment
{
    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public string TypeCode { get; private set; } = null!;
    public string StatusCode { get; private set; } = null!;
    public DateTime StartsAt { get; private set; }
    public DateTime EndsAt { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Notes { get; private set; }
    public int? HealthInstitutionId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public AppointmentType Type { get; private set; } = null!;
    public AppointmentStatus Status { get; private set; } = null!;

    private Appointment()
    {
    }

    public static Appointment Create(
        Guid id,
        Guid patientId,
        Guid doctorId,
        string typeCode,
        DateTime startsAt,
        DateTime endsAt,
        string title,
        DateTime createdAt,
        string? notes = null,
        int? healthInstitutionId = null,
        string statusCode = AppointmentStatusCodes.Scheduled)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Appointment id is required.", nameof(id));

        if (patientId == Guid.Empty)
            throw new ArgumentException("Patient id is required.", nameof(patientId));

        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor id is required.", nameof(doctorId));

        EnsureValidInterval(startsAt, endsAt);

        return new Appointment
        {
            Id = id,
            PatientId = patientId,
            DoctorId = doctorId,
            TypeCode = Guard.Code(typeCode, nameof(typeCode)),
            StatusCode = Guard.Code(statusCode, nameof(statusCode)),
            StartsAt = startsAt,
            EndsAt = endsAt,
            Title = Guard.MaxLength(Guard.NotEmpty(title, nameof(title)), nameof(title), 120),
            Notes = string.IsNullOrWhiteSpace(notes)
                ? null
                : Guard.MaxLength(notes.Trim(), nameof(notes), 512),
            HealthInstitutionId = healthInstitutionId,
            CreatedAt = createdAt
        };
    }

    public void Reschedule(DateTime startsAt, DateTime endsAt, string title, string? notes)
    {
        EnsureActive();
        EnsureValidInterval(startsAt, endsAt);

        StartsAt = startsAt;
        EndsAt = endsAt;
        Title = Guard.MaxLength(Guard.NotEmpty(title, nameof(title)), nameof(title), 120);
        Notes = string.IsNullOrWhiteSpace(notes)
            ? null
            : Guard.MaxLength(notes.Trim(), nameof(notes), 512);
    }

    public void Cancel(DateTime cancelledAt)
    {
        EnsureActive();
        StatusCode = AppointmentStatusCodes.Cancelled;
        CancelledAt = cancelledAt;
    }

    public void Complete(DateTime completedAt)
    {
        EnsureActive();
        StatusCode = AppointmentStatusCodes.Completed;
        CompletedAt = completedAt;
    }

    public bool Overlaps(DateTime startsAt, DateTime endsAt) =>
        StatusCode != AppointmentStatusCodes.Cancelled
        && StartsAt < endsAt
        && startsAt < EndsAt;

    private void EnsureActive()
    {
        if (StatusCode == AppointmentStatusCodes.Cancelled)
            throw new InvalidOperationException("Cancelled appointments cannot be changed.");

        if (StatusCode == AppointmentStatusCodes.Completed)
            throw new InvalidOperationException("Completed appointments cannot be changed.");
    }

    private static void EnsureValidInterval(DateTime startsAt, DateTime endsAt)
    {
        if (endsAt <= startsAt)
            throw new ArgumentException("End time must be after start time.", nameof(endsAt));
    }
}
