using NotificationService.Domain;

namespace NotificationService.Infrastructure.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(this DbContext context)
    {
        await context.SeedNotificationTypesAsync();
        await context.SeedNotificationSeveritiesAsync();
        await context.SeedNotificationChannelsAsync();
        await context.SeedPreferencesAsync();
        await context.SeedNotificationsAsync();
    }

    public static async Task SeedNotificationTypesAsync(this DbContext context)
    {
        var existing = await context.Set<NotificationType>().Select(item => item.Code).ToListAsync();
        var items = new List<NotificationType>
        {
            NotificationType.Create(NotificationTypeCodes.Tumor, "Tumor analysis", "Tumor detection and analysis status"),
            NotificationType.Create(NotificationTypeCodes.Lab, "Lab", "Laboratory results and alerts"),
            NotificationType.Create(NotificationTypeCodes.Medication, "Medication", "Medication conflicts and dosage alerts"),
            NotificationType.Create(NotificationTypeCodes.Security, "Security", "Authentication and access alerts"),
            NotificationType.Create(NotificationTypeCodes.System, "System", "Platform and infrastructure events"),
            NotificationType.Create(NotificationTypeCodes.Radiology, "Radiology", "Imaging and PACS connectivity"),
            NotificationType.Create(NotificationTypeCodes.Appointment, "Appointment", "Appointment scheduling updates")
        }.Where(item => !existing.Contains(item.Code)).ToList();

        if (items.Count == 0)
            return;

        await context.Set<NotificationType>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedNotificationSeveritiesAsync(this DbContext context)
    {
        if (await context.Set<NotificationSeverity>().AnyAsync())
            return;

        var items = new List<NotificationSeverity>
        {
            NotificationSeverity.Create(NotificationSeverityCodes.Critical, "Critical", "Requires immediate attention"),
            NotificationSeverity.Create(NotificationSeverityCodes.Warning, "Warning", "Requires timely review"),
            NotificationSeverity.Create(NotificationSeverityCodes.Info, "Info", "Informational update")
        };

        await context.Set<NotificationSeverity>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedNotificationChannelsAsync(this DbContext context)
    {
        if (await context.Set<NotificationChannel>().AnyAsync())
            return;

        var items = new List<NotificationChannel>
        {
            NotificationChannel.Create(NotificationChannelCodes.InApp, "In-app", "Inbox and header bell"),
            NotificationChannel.Create(NotificationChannelCodes.Email, "Email", "Email via MailService")
        };

        await context.Set<NotificationChannel>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static readonly Guid LoginPatientId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static readonly Guid AssignedDoctorId =
        Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1");

    private static readonly Guid[] SeedUserIds =
    [
        Guid.Parse("11111111-1111-1111-1111-111111111111"),
        LoginPatientId,
        Guid.Parse("33333333-3333-3333-3333-333333333333"),
        Guid.Parse("44444444-4444-4444-4444-444444444444"),
        Guid.Parse("55555555-5555-5555-5555-555555555555"),
        Guid.Parse("66666666-6666-6666-6666-666666666666"),
        Guid.Parse("77777777-7777-7777-7777-777777777777"),
        Guid.Parse("88888888-8888-8888-8888-888888888888"),
        AssignedDoctorId,
        Guid.Parse("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"),
        Guid.Parse("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"),
        Guid.Parse("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"),
        Guid.Parse("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5"),
        Guid.Parse("f6f6f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6"),
        Guid.Parse("a7a7a7a7-a7a7-a7a7-a7a7-a7a7a7a7a7a7"),
        Guid.Parse("b8b8b8b8-b8b8-b8b8-b8b8-b8b8b8b8b8b8")
    ];

    public static async Task SeedPreferencesAsync(this DbContext context)
    {
        if (await context.Set<NotificationPreference>().AnyAsync())
            return;

        var items =
            from userId in SeedUserIds
            from type in NotificationTypeCodes.All
            from channel in NotificationChannelCodes.All
            select NotificationPreference.Create(userId, type, channel);

        await context.Set<NotificationPreference>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedNotificationsAsync(this DbContext context)
    {
        if (await context.Set<Notification>().AnyAsync())
            return;

        var now = new DateTime(2026, 8, 23, 12, 0, 0);
        var items = new List<Notification>();

        foreach (var userId in SeedUserIds)
            items.AddRange(CreateInboxForUser(userId, now, full: userId == LoginPatientId || userId == AssignedDoctorId));

        await context.Set<Notification>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    private static IEnumerable<Notification> CreateInboxForUser(Guid userId, DateTime now, bool full)
    {
        var samples = full
            ? new (string Type, string Severity, string Title, string Message, int MinutesAgo)[]
            {
                (NotificationTypeCodes.Tumor, NotificationSeverityCodes.Critical, "Tumor analysis", "Critical findings ready for review.", 2),
                (NotificationTypeCodes.Lab, NotificationSeverityCodes.Warning, "Lab System", "Abnormal potassium level (6.8 mmol/L).", 10),
                (NotificationTypeCodes.Medication, NotificationSeverityCodes.Critical, "Medication Service", "Dosage conflict detected for an assigned patient.", 15),
                (NotificationTypeCodes.Security, NotificationSeverityCodes.Critical, "Security Alert", "Multiple failed login attempts on the account.", 25),
                (NotificationTypeCodes.Radiology, NotificationSeverityCodes.Warning, "Radiology API", "Connection lost to PACS server.", 40),
                (NotificationTypeCodes.System, NotificationSeverityCodes.Info, "System", "Nightly backup completed successfully.", 60)
            }
            : new (string Type, string Severity, string Title, string Message, int MinutesAgo)[]
            {
                (NotificationTypeCodes.System, NotificationSeverityCodes.Info, "Welcome to NeuroVision", "Your account is ready.", 90),
                (NotificationTypeCodes.Tumor, NotificationSeverityCodes.Info, "Tumor analysis", "A new analysis was queued.", 120)
            };

        foreach (var sample in samples)
        {
            yield return Notification.Create(
                Guid.NewGuid(),
                userId,
                sample.Type,
                sample.Severity,
                sample.Title,
                sample.Message,
                now.AddMinutes(-sample.MinutesAgo));
        }
    }
}
