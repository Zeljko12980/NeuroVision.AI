using AppointmentService.Application.Common;
using BuildingBlocks.Auth;

namespace AppointmentService.UnitTests;

internal static class AppointmentFactory
{
    public static readonly Guid DefaultId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid PatientId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid DoctorId = Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1");
    public static readonly DateTime CreatedAt = new(2026, 8, 23, 12, 0, 0, DateTimeKind.Utc);
    public static readonly DateTime StartsAt = new(2026, 8, 24, 9, 0, 0, DateTimeKind.Utc);
    public static readonly DateTime EndsAt = new(2026, 8, 24, 9, 30, 0, DateTimeKind.Utc);

    public static AppointmentActor PatientActor => new(PatientId, AuthRoles.Patient);
    public static AppointmentActor DoctorActor => new(DoctorId, AuthRoles.Doctor);
    public static AppointmentActor AdminActor => new(Guid.Parse("99999999-9999-9999-9999-999999999999"), AuthRoles.SuperAdministrator);
    public static AppointmentActor OtherPatientActor => new(Guid.Parse("11111111-1111-1111-1111-111111111111"), AuthRoles.Patient);

    public static Appointment Create(
        Guid? id = null,
        Guid? patientId = null,
        Guid? doctorId = null,
        string typeCode = AppointmentTypeCodes.Consultation,
        DateTime? startsAt = null,
        DateTime? endsAt = null,
        string title = "Consultation",
        string? notes = "Seeded appointment",
        DateTime? createdAt = null)
    {
        return Appointment.Create(
            id ?? DefaultId,
            patientId ?? PatientId,
            doctorId ?? DoctorId,
            typeCode,
            startsAt ?? StartsAt,
            endsAt ?? EndsAt,
            title,
            createdAt ?? CreatedAt,
            notes);
    }
}
