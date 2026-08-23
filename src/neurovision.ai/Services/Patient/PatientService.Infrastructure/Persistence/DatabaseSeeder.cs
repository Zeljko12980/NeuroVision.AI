using PatientService.Domain;

namespace PatientService.Infrastructure.Seeding;

public static class DatabaseSeeder
{
    private static readonly Guid AssignedDoctorId =
        Guid.Parse("a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1");

    public static async Task SeedAsync(this DbContext context)
    {
        await context.SeedPatientStatusesAsync();
        await context.SeedGendersAsync();
        await context.SeedBloodTypesAsync();
        await context.SeedLanguagesAsync();
        await context.SeedAllergiesAsync();
        await context.SeedConditionsAsync();
        await context.SeedInsurancePayersAsync();
        await context.SeedRelationshipTypesAsync();
        await context.SeedConsentTypesAsync();
        await context.SeedPatientsAsync();
    }

    public static async Task SeedPatientStatusesAsync(this DbContext context)
    {
        if (await context.Set<PatientStatus>().AnyAsync())
            return;

        var items = new List<PatientStatus>
        {
            PatientStatus.Create(PatientStatusCodes.PendingVerification, "Pending verification", "Patient created, waiting for activation"),
            PatientStatus.Create(PatientStatusCodes.Active, "Active", "Patient can use the platform"),
            PatientStatus.Create(PatientStatusCodes.Inactive, "Inactive", "Temporarily inactive"),
            PatientStatus.Create(PatientStatusCodes.Deceased, "Deceased", "Patient marked as deceased"),
            PatientStatus.Create(PatientStatusCodes.Archived, "Archived", "Archived clinical record")
        };

        await context.Set<PatientStatus>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedGendersAsync(this DbContext context)
    {
        if (await context.Set<Gender>().AnyAsync())
            return;

        var items = new List<Gender>
        {
            Gender.Create(GenderCodes.Male, "Male"),
            Gender.Create(GenderCodes.Female, "Female"),
            Gender.Create(GenderCodes.Other, "Other"),
            Gender.Create(GenderCodes.Unknown, "Unknown")
        };

        await context.Set<Gender>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedBloodTypesAsync(this DbContext context)
    {
        if (await context.Set<BloodType>().AnyAsync())
            return;

        var items = new List<BloodType>
        {
            BloodType.Create("AP", "A+"),
            BloodType.Create("AN", "A-"),
            BloodType.Create("BP", "B+"),
            BloodType.Create("BN", "B-"),
            BloodType.Create("ABP", "AB+"),
            BloodType.Create("ABN", "AB-"),
            BloodType.Create("OP", "O+"),
            BloodType.Create("ON", "O-")
        };

        await context.Set<BloodType>().AddRangeAsync(items);
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

    public static async Task SeedAllergiesAsync(this DbContext context)
    {
        if (await context.Set<Allergy>().AnyAsync())
            return;

        var items = new List<Allergy>
        {
            Allergy.Create("PNC", "Penicillin", "Beta-lactam antibiotic allergy"),
            Allergy.Create("LATEX", "Latex", "Natural rubber latex allergy"),
            Allergy.Create("IOD", "Iodine", "Iodinated contrast allergy"),
            Allergy.Create("NSAID", "NSAID", "Non-steroidal anti-inflammatory drug allergy"),
            Allergy.Create("PEAN", "Peanut", "Peanut food allergy")
        };

        await context.Set<Allergy>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedConditionsAsync(this DbContext context)
    {
        if (await context.Set<Condition>().AnyAsync())
            return;

        var items = new List<Condition>
        {
            Condition.Create("TUM", "Brain tumor", "Intracranial neoplasm"),
            Condition.Create("EPI", "Epilepsy", "Recurrent unprovoked seizures"),
            Condition.Create("MS", "Multiple sclerosis", "Central nervous system demyelination"),
            Condition.Create("MIG", "Migraine", "Primary headache disorder"),
            Condition.Create("STRK", "Stroke", "Cerebrovascular accident"),
            Condition.Create("PARK", "Parkinson disease", "Neurodegenerative movement disorder")
        };

        await context.Set<Condition>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedInsurancePayersAsync(this DbContext context)
    {
        if (await context.Set<InsurancePayer>().AnyAsync())
            return;

        var items = new List<InsurancePayer>
        {
            InsurancePayer.Create("FBIH", "FZZO FBiH", "Fond zdravstvenog osiguranja Federacije BiH"),
            InsurancePayer.Create("RS", "FZO RS", "Fond zdravstvenog osiguranja Republike Srpske"),
            InsurancePayer.Create("BD", "FZO BD", "Fond zdravstvenog osiguranja Brčko Distrikta"),
            InsurancePayer.Create("PRIV", "Private", "Private health insurance"),
            InsurancePayer.Create("EU", "EU coverage", "European health insurance coverage")
        };

        await context.Set<InsurancePayer>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedRelationshipTypesAsync(this DbContext context)
    {
        if (await context.Set<RelationshipType>().AnyAsync())
            return;

        var items = new List<RelationshipType>
        {
            RelationshipType.Create("SPOU", "Spouse"),
            RelationshipType.Create("PAR", "Parent"),
            RelationshipType.Create("CHILD", "Child"),
            RelationshipType.Create("SIB", "Sibling"),
            RelationshipType.Create("FRIEND", "Friend"),
            RelationshipType.Create("OTHER", "Other")
        };

        await context.Set<RelationshipType>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedConsentTypesAsync(this DbContext context)
    {
        if (await context.Set<ConsentType>().AnyAsync())
            return;

        var items = new List<ConsentType>
        {
            ConsentType.Create("DATA", "Data processing", "Processing of personal and clinical data"),
            ConsentType.Create("IMG", "Imaging", "Storage and review of medical images"),
            ConsentType.Create("SHARE", "Data sharing", "Sharing records with assigned clinicians"),
            ConsentType.Create("AI", "AI analysis", "Use of AI models on clinical data")
        };

        await context.Set<ConsentType>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public static async Task SeedPatientsAsync(this DbContext context)
    {
        const string loginEmail = "armanigas78@gmail.com";
        var loginId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        if (await context.Set<Patient>().AnyAsync())
        {
            var loginPatient = await context.Set<Patient>().FindAsync(loginId)
                ?? await context.Set<Patient>().OrderBy(item => item.CreatedAt).FirstAsync();

            if (!string.Equals(loginPatient.Email, loginEmail, StringComparison.OrdinalIgnoreCase))
                loginPatient.UpdateContact(loginEmail, loginPatient.Phone);

            await AssignSeedPicturesAsync(context);
            await context.SaveChangesAsync();
            return;
        }

        var createdAt = new DateTime(2024, 3, 1);
        var items = new List<Patient>
        {
            CreateSeedPatient(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "Amila", "Kovačević", "amila.kovacevic@neurovision.ai", "+38761111222",
                new DateTime(1988, 4, 12), GenderCodes.Female, "AP", "1904988123456",
                "Follow-up after glioma resection.", "Klinički centar Sarajevo", 1,
                AssignedDoctorId, "Zmaja od Bosne 12", ["BS", "EN"], ["PNC"],
                [("TUM", 2022)], "FBIH", "POL-FBIH-1001", "Lejla Kovačević", "+38761111333", "PAR",
                createdAt),
            CreateSeedPatient(
                loginId,
                "Haris", "Delić", loginEmail, "+38762222333",
                new DateTime(1975, 9, 3), GenderCodes.Male, "OP", "0309975123456",
                "Drug-resistant focal epilepsy.", "Klinički centar Sarajevo", 1,
                AssignedDoctorId, "Titova 8", ["BS", "EN"], ["IOD"],
                [("EPI", 2018)], "FBIH", "POL-FBIH-1002", "Aida Delić", "+38762222444", "SPOU",
                createdAt),
            CreateSeedPatient(
                Guid.Parse("33333333-3333-3333-3333-333333333333"),
                "Milica", "Janković", "milica.jankovic@neurovision.ai", "+38765333444",
                new DateTime(1991, 1, 21), GenderCodes.Female, "BP", "2101991123456",
                "Relapsing-remitting multiple sclerosis.", "UKC Banja Luka", 2,
                AssignedDoctorId, "Kralja Petra 4", ["SR", "EN"], ["LATEX"],
                [("MS", 2019)], "RS", "POL-RS-2001", "Petar Janković", "+38765333555", "SIB",
                createdAt),
            CreateSeedPatient(
                Guid.Parse("44444444-4444-4444-4444-444444444444"),
                "Ivan", "Marić", "ivan.maric@neurovision.ai", "+38766444555",
                new DateTime(1968, 11, 7), GenderCodes.Male, "ABP", "0711968123456",
                "Chronic migraine with aura.", "KBC Mostar", 3,
                AssignedDoctorId, "Kneza Višeslava 19", ["HR", "EN"], ["NSAID"],
                [("MIG", 2015)], "FBIH", "POL-FBIH-1003", "Ana Marić", "+38766444666", "SPOU",
                createdAt),
            CreateSeedPatient(
                Guid.Parse("55555555-5555-5555-5555-555555555555"),
                "Selma", "Hadžić", "selma.hadzic@neurovision.ai", "+38767555666",
                new DateTime(1959, 6, 18), GenderCodes.Female, "AN", "1806959123456",
                "Ischemic stroke rehabilitation.", "Klinički centar Sarajevo", 1,
                AssignedDoctorId, "Hamdije Čemerlića 2", ["BS"], ["PNC", "IOD"],
                [("STRK", 2023)], "FBIH", "POL-FBIH-1004", "Emir Hadžić", "+38767555777", "CHILD",
                createdAt),
            CreateSeedPatient(
                Guid.Parse("66666666-6666-6666-6666-666666666666"),
                "Nenad", "Stanković", "nenad.stankovic@neurovision.ai", "+38768666777",
                new DateTime(1952, 2, 27), GenderCodes.Male, "ON", "2702952123456",
                "Parkinson disease motor follow-up.", "UKC Banja Luka", 2,
                AssignedDoctorId, "Jovana Dučića 11", ["SR", "EN"], null,
                [("PARK", 2020)], "RS", "POL-RS-2002", "Mira Stanković", "+38768666888", "SPOU",
                createdAt),
            CreateSeedPatient(
                Guid.Parse("77777777-7777-7777-7777-777777777777"),
                "Emina", "Begić", "emina.begic@neurovision.ai", "+38769777888",
                new DateTime(2001, 8, 14), GenderCodes.Female, "BN", "1408001123456",
                "First seizure workup.", "Klinički centar Sarajevo", 1,
                AssignedDoctorId, "Grbavička 5", ["BS", "EN", "DE"], ["PEAN"],
                [("EPI", 2024)], "PRIV", "POL-PRIV-3001", "Amra Begić", "+38769777999", "PAR",
                createdAt),
            CreateSeedPatient(
                Guid.Parse("88888888-8888-8888-8888-888888888888"),
                "Luka", "Horvat", "luka.horvat@neurovision.ai", "+38760888999",
                new DateTime(1984, 12, 2), GenderCodes.Male, "ABN", "0212984123456",
                "Post-operative meningioma surveillance.", "KBC Mostar", 3,
                AssignedDoctorId, "Šetalište 1", ["HR", "EN", "DE"], ["LATEX"],
                [("TUM", 2021), ("MIG", 2016)], "EU", "POL-EU-4001", "Ivana Horvat", "+38760888000", "SPOU",
                createdAt)
        };

        await context.Set<Patient>().AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    private static readonly IReadOnlyDictionary<Guid, string> SeedPictures =
        new Dictionary<Guid, string>
        {
            [Guid.Parse("11111111-1111-1111-1111-111111111111")] = "patients/11111111-1111-1111-1111-111111111111.jpg",
            [Guid.Parse("22222222-2222-2222-2222-222222222222")] = "patients/22222222-2222-2222-2222-222222222222.jpg",
            [Guid.Parse("33333333-3333-3333-3333-333333333333")] = "patients/33333333-3333-3333-3333-333333333333.jpg",
            [Guid.Parse("44444444-4444-4444-4444-444444444444")] = "patients/44444444-4444-4444-4444-444444444444.jpg",
            [Guid.Parse("55555555-5555-5555-5555-555555555555")] = "patients/55555555-5555-5555-5555-555555555555.jpg",
            [Guid.Parse("66666666-6666-6666-6666-666666666666")] = "patients/66666666-6666-6666-6666-666666666666.jpg",
            [Guid.Parse("77777777-7777-7777-7777-777777777777")] = "patients/77777777-7777-7777-7777-777777777777.jpg",
            [Guid.Parse("88888888-8888-8888-8888-888888888888")] = "patients/88888888-8888-8888-8888-888888888888.jpg"
        };

    private static async Task AssignSeedPicturesAsync(DbContext context)
    {
        var patients = await context.Set<Patient>().ToListAsync();
        foreach (var patient in patients)
        {
            if (!string.IsNullOrWhiteSpace(patient.ProfilePictureUrl))
                continue;

            if (SeedPictures.TryGetValue(patient.Id, out var pictureUrl))
                patient.UpdateProfile(pictureUrl, patient.Notes);
        }
    }

    private static string? SeedPictureUrl(Guid id) =>
        SeedPictures.TryGetValue(id, out var url) ? url : null;

    private static Patient CreateSeedPatient(
        Guid id,
        string firstName,
        string lastName,
        string email,
        string phone,
        DateTime dateOfBirth,
        string genderCode,
        string bloodTypeCode,
        string nationalId,
        string notes,
        string institutionName,
        int healthInstitutionId,
        Guid assignedDoctorId,
        string addressLine,
        string[] languages,
        string[]? allergies,
        (string Code, int Year)[] conditions,
        string insurancePayerCode,
        string insurancePolicyNumber,
        string emergencyName,
        string emergencyPhone,
        string emergencyRelationship,
        DateTime createdAt)
    {
        var patient = Patient.Create(
            id,
            firstName,
            lastName,
            email,
            phone,
            dateOfBirth,
            genderCode,
            PatientStatusCodes.Active,
            bloodTypeCode,
            nationalId,
            profilePictureUrl: SeedPictureUrl(id),
            notes,
            healthInstitutionId,
            institutionName,
            assignedDoctorId,
            addressLine,
            settlementId: 1,
            municipalityId: 1,
            countryId: 1,
            heightCm: 170,
            weightKg: 70,
            createdAt);

        foreach (var language in languages)
            patient.AddLanguage(language);

        if (allergies is not null)
        {
            foreach (var allergy in allergies)
                patient.AddAllergy(allergy);
        }

        foreach (var condition in conditions)
            patient.AddCondition(condition.Code, condition.Year);

        patient.ChangeInsurance(insurancePayerCode, insurancePolicyNumber, createdAt);
        patient.AddEmergencyContact(emergencyName, emergencyPhone, emergencyRelationship);
        patient.GrantConsent("DATA", createdAt);
        patient.GrantConsent("IMG", createdAt);
        patient.GrantConsent("SHARE", createdAt);

        return patient;
    }
}
