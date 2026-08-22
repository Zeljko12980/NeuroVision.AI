using DoctorService.Domain;
using DoctorService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DoctorService.Infrastructure.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(this DbContext context)
    {
        await context.SeedDoctorStatusesAsync();
        await context.SeedSpecializationsAsync();
        await context.SeedLanguagesAsync();
        await context.SeedDegreeTypesAsync();
        await context.SeedLicenseAuthoritiesAsync();
        await context.SeedDoctorsAsync();
    }

    public static async Task SeedDoctorStatusesAsync(this DbContext context)
    {
        if (await context.Set<DoctorStatus>().AnyAsync())
            return;

        var items = new List<DoctorStatus>
        {
            DoctorStatus.Create(DoctorStatusCodes.PendingVerification, "Pending verification", "Doctor created, waiting for activation"),
            DoctorStatus.Create(DoctorStatusCodes.Active, "Active", "Doctor can use the platform"),
            DoctorStatus.Create(DoctorStatusCodes.Suspended, "Suspended", "Temporarily blocked"),
            DoctorStatus.Create(DoctorStatusCodes.Deactivated, "Deactivated", "Permanently deactivated")
        };

        await context.Set<DoctorStatus>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedSpecializationsAsync(this DbContext context)
    {
        if (await context.Set<Specialization>().AnyAsync())
            return;

        var items = new List<Specialization>
        {
            Specialization.Create("NEUR", "Neurology", "Clinical neurology"),
            Specialization.Create("NRAD", "Neuroradiology", "Diagnostic neuroradiology"),
            Specialization.Create("NONC", "Neuro-oncology", "Brain and CNS tumors"),
            Specialization.Create("NSRG", "Neurosurgery", "Surgical treatment of neurological disease"),
            Specialization.Create("NINT", "Interventional neuroradiology", "Endovascular neuroimaging procedures")
        };

        await context.Set<Specialization>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedLanguagesAsync(this DbContext context)
    {
        if (await context.Set<Language>().AnyAsync())
            return;

        var items = new List<Language>
        {
            Language.Create("BS", "Bosanski"),
            Language.Create("EN", "English"),
            Language.Create("DE", "Deutsch"),
            Language.Create("HR", "Hrvatski"),
            Language.Create("SR", "Srpski")
        };

        await context.Set<Language>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedDegreeTypesAsync(this DbContext context)
    {
        if (await context.Set<DegreeType>().AnyAsync())
            return;

        var items = new List<DegreeType>
        {
            DegreeType.Create("MD", "Doctor of Medicine"),
            DegreeType.Create("PHD", "Doctor of Philosophy"),
            DegreeType.Create("SPEC", "Specialist"),
            DegreeType.Create("MSC", "Master of Science"),
            DegreeType.Create("PROF", "Professor")
        };

        await context.Set<DegreeType>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedLicenseAuthoritiesAsync(this DbContext context)
    {
        if (await context.Set<LicenseAuthority>().AnyAsync())
            return;

        var items = new List<LicenseAuthority>
        {
            LicenseAuthority.Create("FBIH", "Federacija BiH"),
            LicenseAuthority.Create("RS", "Republika Srpska"),
            LicenseAuthority.Create("BD", "Brčko Distrikt"),
            LicenseAuthority.Create("EU", "European Union")
        };

        await context.Set<LicenseAuthority>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedDoctorsAsync(this DbContext context)
    {
        const string loginEmail = "ikanoviczeljko362@gmail.com";

        if (await context.Set<Doctor>().AnyAsync())
        {
            var hasLoginEmail = await context.Set<Doctor>()
                .AnyAsync(doctor => doctor.Email == loginEmail);

            if (!hasLoginEmail)
            {
                var doctor = await context.Set<Doctor>()
                    .OrderBy(item => item.CreatedAt)
                    .FirstAsync();

                doctor.UpdateContact(loginEmail, doctor.Phone);
            }

            await AssignSeedPicturesAsync(context);
            await context.SaveChangesAsync();
            return;
        }

        var createdAt = new DateTime(2024, 1, 15);
        var items = new List<Doctor>
        {
            CreateSeedDoctor(
                Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"),
                "Zeljko", "Ikanovic", loginEmail, "+38761222333",
                "LIC-BA-1001", "FBIH", "NEUR", "Klinički centar Sarajevo", 1,
                ["BS", "EN"], ["MD", "SPEC"], createdAt),
            CreateSeedDoctor(
                Guid.Parse("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"),
                "Marko", "Petrović", "marko.petrovic@neurovision.ai", "+38164111222",
                "LIC-RS-2002", "RS", "NRAD", "Klinički centar Srbije", 2,
                ["SR", "EN"], ["MD", "PHD"], createdAt),
            CreateSeedDoctor(
                Guid.Parse("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3"),
                "Ivana", "Horvat", "ivana.horvat@neurovision.ai", "+385911234567",
                "LIC-HR-3003", "EU", "NONC", "KBC Zagreb", 3,
                ["HR", "EN", "DE"], ["MD", "SPEC", "PHD"], createdAt),
            CreateSeedDoctor(
                Guid.Parse("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4"),
                "Nikola", "Jovanović", "nikola.jovanovic@neurovision.ai", "+38267222333",
                "LIC-ME-4004", "EU", "NSRG", "Klinički centar Crne Gore", 4,
                ["SR", "EN"], ["MD", "SPEC"], createdAt),
            CreateSeedDoctor(
                Guid.Parse("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5"),
                "Sara", "Begović", "sara.begovic@neurovision.ai", "+38763333444",
                "LIC-BA-5005", "FBIH", "NINT", "Klinički centar Sarajevo", 1,
                ["BS", "EN", "DE"], ["MD", "MSC"], createdAt),
            CreateSeedDoctor(
                Guid.Parse("f6f6f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6"),
                "Luka", "Novak", "luka.novak@neurovision.ai", "+38640111222",
                "LIC-SI-6006", "EU", "NRAD", "UKC Ljubljana", 5,
                ["EN", "DE"], ["MD", "PHD"], createdAt),
            CreateSeedDoctor(
                Guid.Parse("a7a7a7a7-a7a7-a7a7-a7a7-a7a7a7a7a7a7"),
                "Maja", "Stojanovska", "maja.stojanovska@neurovision.ai", "+38970123456",
                "LIC-MK-7007", "EU", "NEUR", "Klinički centar Skoplje", 6,
                ["EN"], ["MD", "SPEC"], createdAt),
            CreateSeedDoctor(
                Guid.Parse("b8b8b8b8-b8b8-b8b8-b8b8-b8b8b8b8b8b8"),
                "Emir", "Hadžić", "emir.hadzic@neurovision.ai", "+38765444555",
                "LIC-BA-8008", "FBIH", "NSRG", "Klinički centar Sarajevo", 1,
                ["BS", "EN"], ["MD", "SPEC", "PROF"], createdAt),
        };

        items[0].AddWorkingSlot(1, new TimeSpan(8, 0, 0), new TimeSpan(16, 0, 0), createdAt);
        items[1].AddWorkingSlot(2, new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), createdAt);
        items[2].AddWorkingSlot(3, new TimeSpan(8, 0, 0), new TimeSpan(14, 0, 0), createdAt);

        await context.Set<Doctor>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    private static readonly IReadOnlyDictionary<Guid, string> SeedPictures =
        new Dictionary<Guid, string>
        {
            [Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1")] = "doctors/a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1.jpg",
            [Guid.Parse("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2")] = "doctors/b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2.jpg",
            [Guid.Parse("c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3")] = "doctors/c3c3c3c3-c3c3-c3c3-c3c3-c3c3c3c3c3c3.jpg",
            [Guid.Parse("d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4")] = "doctors/d4d4d4d4-d4d4-d4d4-d4d4-d4d4d4d4d4d4.jpg",
            [Guid.Parse("e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5")] = "doctors/e5e5e5e5-e5e5-e5e5-e5e5-e5e5e5e5e5e5.jpg",
            [Guid.Parse("f6f6f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6")] = "doctors/f6f6f6f6-f6f6-f6f6-f6f6-f6f6f6f6f6f6.jpg",
            [Guid.Parse("a7a7a7a7-a7a7-a7a7-a7a7-a7a7a7a7a7a7")] = "doctors/a7a7a7a7-a7a7-a7a7-a7a7-a7a7a7a7a7a7.jpg",
            [Guid.Parse("b8b8b8b8-b8b8-b8b8-b8b8-b8b8b8b8b8b8")] = "doctors/b8b8b8b8-b8b8-b8b8-b8b8-b8b8b8b8b8b8.jpg"
        };

    private static async Task AssignSeedPicturesAsync(DbContext context)
    {
        var doctors = await context.Set<Doctor>().ToListAsync();
        foreach (var doctor in doctors)
        {
            if (!string.IsNullOrWhiteSpace(doctor.ProfilePictureUrl))
                continue;

            if (SeedPictures.TryGetValue(doctor.Id, out var pictureUrl))
                doctor.UpdateProfile(pictureUrl, doctor.Bio);
        }
    }

    private static string? SeedPictureUrl(Guid id) =>
        SeedPictures.TryGetValue(id, out var url) ? url : null;

    private static Doctor CreateSeedDoctor(
        Guid id,
        string firstName,
        string lastName,
        string email,
        string phone,
        string licenseNumber,
        string licenseAuthorityCode,
        string specializationCode,
        string institutionName,
        int healthInstitutionId,
        string[] languages,
        string[] degrees,
        DateTime createdAt)
    {
        var doctor = Doctor.Create(
            id,
            firstName,
            lastName,
            email,
            phone,
            licenseNumber,
            licenseAuthorityCode,
            specializationCode,
            DoctorStatusCodes.Active,
            profilePictureUrl: SeedPictureUrl(id),
            bio: $"{specializationCode} specialist at {institutionName}.",
            healthInstitutionId,
            institutionName,
            isAvailable: true,
            createdAt);

        foreach (var language in languages)
            doctor.AddLanguage(language);

        foreach (var degree in degrees)
            doctor.AddDegree(degree);

        return doctor;
    }
}
