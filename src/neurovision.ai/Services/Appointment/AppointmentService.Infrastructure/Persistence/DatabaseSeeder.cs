namespace AppointmentService.Infrastructure.Seeding;

public static class DatabaseSeeder
{
    public static readonly Guid LoginPatientId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static readonly Guid AssignedDoctorId =
        Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1");

    private static readonly Guid AmilaId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid MilicaId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid IvanId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private static readonly Guid SelmaId = Guid.Parse("55555555-5555-5555-5555-555555555555");
    private static readonly Guid NenadId = Guid.Parse("66666666-6666-6666-6666-666666666666");
    private static readonly Guid EminaId = Guid.Parse("77777777-7777-7777-7777-777777777777");
    private static readonly Guid LukaPatientId = Guid.Parse("88888888-8888-8888-8888-888888888888");
    private static readonly Guid MarkoDoctorId = Guid.Parse("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2");

    public static async Task SeedAsync(this DbContext context)
    {
        await context.SeedAppointmentTypesAsync();
        await context.SeedAppointmentStatusesAsync();
        await context.SeedAppointmentsAsync();
    }

    public static async Task SeedAppointmentTypesAsync(this DbContext context)
    {
        if (await context.Set<AppointmentType>().AnyAsync())
            return;

        var items = new List<AppointmentType>
        {
            AppointmentType.Create(AppointmentTypeCodes.Consultation, "Consultation", "Initial or general consultation"),
            AppointmentType.Create(AppointmentTypeCodes.FollowUp, "Follow-up", "Follow-up visit"),
            AppointmentType.Create(AppointmentTypeCodes.Scan, "Scan", "Imaging and scan appointment")
        };

        await context.Set<AppointmentType>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedAppointmentStatusesAsync(this DbContext context)
    {
        if (await context.Set<AppointmentStatus>().AnyAsync())
            return;

        var items = new List<AppointmentStatus>
        {
            AppointmentStatus.Create(AppointmentStatusCodes.Scheduled, "Scheduled", "Upcoming appointment"),
            AppointmentStatus.Create(AppointmentStatusCodes.Cancelled, "Cancelled", "Cancelled appointment"),
            AppointmentStatus.Create(AppointmentStatusCodes.Completed, "Completed", "Completed appointment")
        };

        await context.Set<AppointmentStatus>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedAppointmentsAsync(this DbContext context)
    {
        var existingIds = await context.Set<Appointment>().Select(item => item.Id).ToListAsync();
        var items = BuildSeedAppointments()
            .Where(item => !existingIds.Contains(item.Id))
            .ToList();

        if (items.Count == 0)
            return;

        await context.Set<Appointment>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    private static IEnumerable<Appointment> BuildSeedAppointments()
    {
        var today = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Unspecified);
        var createdAt = today.AddDays(-14).AddHours(8);

        var harisMorning = Create(
            Guid.Parse("aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa"),
            LoginPatientId, AssignedDoctorId, AppointmentTypeCodes.Consultation,
            today.AddDays(1).AddHours(9), today.AddDays(1).AddHours(9).AddMinutes(30),
            "Consultation", createdAt, "Follow-up after epilepsy review.", 1);

        var harisAfternoon = Create(
            Guid.Parse("aaaaaaaa-2222-2222-2222-aaaaaaaaaaaa"),
            LoginPatientId, AssignedDoctorId, AppointmentTypeCodes.FollowUp,
            today.AddDays(1).AddHours(14), today.AddDays(1).AddHours(14).AddMinutes(30),
            "Follow-up", createdAt, "Medication review with Dr. Ikanovic.", 1);

        var harisScan = Create(
            Guid.Parse("aaaaaaaa-3333-3333-3333-aaaaaaaaaaaa"),
            LoginPatientId, AssignedDoctorId, AppointmentTypeCodes.Scan,
            today.AddDays(-1).AddHours(10), today.AddDays(-1).AddHours(10).AddMinutes(45),
            "MRI scan", createdAt.AddDays(-2), "Seeded completed scan", 1);
        harisScan.Complete(today.AddDays(-1).AddHours(11));

        var harisCancelled = Create(
            Guid.Parse("aaaaaaaa-4444-4444-4444-aaaaaaaaaaaa"),
            LoginPatientId, AssignedDoctorId, AppointmentTypeCodes.Consultation,
            today.AddDays(7).AddHours(11), today.AddDays(7).AddHours(11).AddMinutes(30),
            "Cancelled consult", createdAt, "Patient asked to postpone.", 1);
        harisCancelled.Cancel(today.AddDays(-1).AddHours(16));

        var amila = Create(
            Guid.Parse("bbbbbbbb-1111-1111-1111-bbbbbbbbbbbb"),
            AmilaId, AssignedDoctorId, AppointmentTypeCodes.FollowUp,
            today.AddDays(2).AddHours(10), today.AddDays(2).AddHours(10).AddMinutes(30),
            "Glioma follow-up", createdAt, "Post-resection review for Amila Kovačević.", 1);

        var milica = Create(
            Guid.Parse("bbbbbbbb-2222-2222-2222-bbbbbbbbbbbb"),
            MilicaId, AssignedDoctorId, AppointmentTypeCodes.Consultation,
            today.AddDays(3).AddHours(11), today.AddDays(3).AddHours(11).AddMinutes(30),
            "MS consultation", createdAt, "Relapsing-remitting MS check-in.", 2);

        var nenad = Create(
            Guid.Parse("bbbbbbbb-3333-3333-3333-bbbbbbbbbbbb"),
            NenadId, AssignedDoctorId, AppointmentTypeCodes.FollowUp,
            today.AddDays(4).AddHours(13), today.AddDays(4).AddHours(13).AddMinutes(30),
            "Parkinson follow-up", createdAt, "Motor symptom follow-up.", 2);

        var emina = Create(
            Guid.Parse("bbbbbbbb-4444-4444-4444-bbbbbbbbbbbb"),
            EminaId, AssignedDoctorId, AppointmentTypeCodes.Consultation,
            today.AddDays(5).AddHours(9), today.AddDays(5).AddHours(9).AddMinutes(30),
            "First seizure workup", createdAt, "New patient consult.", 1);

        var selmaScan = Create(
            Guid.Parse("bbbbbbbb-5555-5555-5555-bbbbbbbbbbbb"),
            SelmaId, AssignedDoctorId, AppointmentTypeCodes.Scan,
            today.AddDays(-3).AddHours(9), today.AddDays(-3).AddHours(9).AddMinutes(45),
            "Stroke imaging", createdAt, "Rehabilitation imaging follow-up.", 1);
        selmaScan.Complete(today.AddDays(-3).AddHours(10));

        var ivanMarko = Create(
            Guid.Parse("cccccccc-1111-1111-1111-cccccccccccc"),
            IvanId, MarkoDoctorId, AppointmentTypeCodes.Consultation,
            today.AddDays(2).AddHours(9), today.AddDays(2).AddHours(9).AddMinutes(30),
            "Migraine consult", createdAt, "Seen by Dr. Petrović.", 3);

        var lukaMarko = Create(
            Guid.Parse("cccccccc-2222-2222-2222-cccccccccccc"),
            LukaPatientId, MarkoDoctorId, AppointmentTypeCodes.Scan,
            today.AddDays(6).AddHours(15), today.AddDays(6).AddHours(15).AddMinutes(45),
            "Meningioma surveillance", createdAt, "Post-operative imaging.", 3);

        return
        [
            harisMorning,
            harisAfternoon,
            harisScan,
            harisCancelled,
            amila,
            milica,
            nenad,
            emina,
            selmaScan,
            ivanMarko,
            lukaMarko
        ];
    }

    private static Appointment Create(
        Guid id,
        Guid patientId,
        Guid doctorId,
        string typeCode,
        DateTime startsAt,
        DateTime endsAt,
        string title,
        DateTime createdAt,
        string notes,
        int healthInstitutionId) =>
        Appointment.Create(
            id,
            patientId,
            doctorId,
            typeCode,
            startsAt,
            endsAt,
            title,
            createdAt,
            notes,
            healthInstitutionId);
}
