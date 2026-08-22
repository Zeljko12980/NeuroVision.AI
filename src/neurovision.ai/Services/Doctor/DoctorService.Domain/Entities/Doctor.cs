namespace DoctorService.Domain.Entities;

public class Doctor
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string LicenseNumber { get; private set; } = null!;
    public string? LicenseAuthorityCode { get; private set; }
    public string CurrentSpecializationCode { get; private set; } = null!;
    public string CurrentStatusCode { get; private set; } = null!;
    public string? ProfilePictureUrl { get; private set; }
    public string? Bio { get; private set; }
    public int? CurrentHealthInstitutionId { get; private set; }
    public string? CurrentInstitutionName { get; private set; }
    public bool IsAvailable { get; private set; }
    public DateTime LastActive { get; private set; }
    public decimal AverageRating { get; private set; }
    public int TotalReviews { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public DoctorStatus Status { get; private set; } = null!;
    public Specialization CurrentSpecialization { get; private set; } = null!;
    public LicenseAuthority? LicenseAuthority { get; private set; }

    public ICollection<DoctorStatusHistory> StatusHistories { get; private set; } = new List<DoctorStatusHistory>();
    public ICollection<DoctorLicenseHistory> LicenseHistories { get; private set; } = new List<DoctorLicenseHistory>();
    public ICollection<DoctorAffiliationHistory> AffiliationHistories { get; private set; } = new List<DoctorAffiliationHistory>();
    public ICollection<DoctorLanguageCoverage> LanguageCoverages { get; private set; } = new List<DoctorLanguageCoverage>();
    public ICollection<DoctorDegreeCoverage> DegreeCoverages { get; private set; } = new List<DoctorDegreeCoverage>();
    public ICollection<DoctorSpecializationCoverage> SpecializationCoverages { get; private set; } = new List<DoctorSpecializationCoverage>();
    public ICollection<WorkingSlot> WorkingSlots { get; private set; } = new List<WorkingSlot>();
    public ICollection<DoctorReview> Reviews { get; private set; } = new List<DoctorReview>();

    private Doctor()
    {
    }

    public static Doctor Create(
        Guid id,
        string firstName,
        string lastName,
        string email,
        string phone,
        string licenseNumber,
        string? licenseAuthorityCode,
        string specializationCode,
        string statusCode,
        string? profilePictureUrl,
        string? bio,
        int? healthInstitutionId,
        string? institutionName,
        bool isAvailable,
        DateTime createdAt)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Doctor id is required.", nameof(id));

        var specialization = Guard.Code(specializationCode, nameof(specializationCode));
        var status = Guard.Code(statusCode, nameof(statusCode));
        var authority = string.IsNullOrWhiteSpace(licenseAuthorityCode)
            ? null
            : Guard.Code(licenseAuthorityCode, nameof(licenseAuthorityCode));

        var doctor = new Doctor
        {
            Id = id,
            FirstName = Guard.NotEmpty(firstName, nameof(firstName)),
            LastName = Guard.NotEmpty(lastName, nameof(lastName)),
            Email = Guard.NotEmpty(email, nameof(email)).ToLowerInvariant(),
            Phone = Guard.NotEmpty(phone, nameof(phone)),
            LicenseNumber = Guard.NotEmpty(licenseNumber, nameof(licenseNumber)),
            LicenseAuthorityCode = authority,
            CurrentSpecializationCode = specialization,
            CurrentStatusCode = status,
            ProfilePictureUrl = EmptyToNull(profilePictureUrl),
            Bio = EmptyToNull(bio),
            CurrentHealthInstitutionId = healthInstitutionId is <= 0 ? null : healthInstitutionId,
            CurrentInstitutionName = EmptyToNull(institutionName),
            IsAvailable = isAvailable,
            LastActive = createdAt,
            AverageRating = 0,
            TotalReviews = 0,
            CreatedAt = createdAt
        };

        doctor.StatusHistories.Add(DoctorStatusHistory.Create(id, 1, status, createdAt));
        doctor.LicenseHistories.Add(DoctorLicenseHistory.Create(id, 1, doctor.LicenseNumber, authority, createdAt));
        doctor.SpecializationCoverages.Add(
            DoctorSpecializationCoverage.Create(id, specialization, isPrimary: true, from: createdAt));

        if (!string.IsNullOrWhiteSpace(doctor.CurrentInstitutionName))
        {
            doctor.AffiliationHistories.Add(DoctorAffiliationHistory.Create(
                id,
                1,
                doctor.CurrentInstitutionName,
                doctor.CurrentHealthInstitutionId,
                createdAt));
        }

        return doctor;
    }

    public void Activate(DateTime at) => ChangeStatus(DoctorStatusCodes.Active, at);

    public void Suspend(DateTime at) => ChangeStatus(DoctorStatusCodes.Suspended, at);

    public void Deactivate(DateTime at) => ChangeStatus(DoctorStatusCodes.Deactivated, at);

    public void UpdateProfile(string? pictureUrl, string? bio)
    {
        if (!string.IsNullOrWhiteSpace(pictureUrl))
            ProfilePictureUrl = pictureUrl.Trim();

        Bio = EmptyToNull(bio) ?? Bio;
    }

    public void SetAvailability(bool isAvailable, DateTime at)
    {
        IsAvailable = isAvailable;
        LastActive = at;
    }

    public void UpdateContact(string email, string phone)
    {
        Email = Guard.NotEmpty(email, nameof(email)).ToLowerInvariant();
        Phone = Guard.NotEmpty(phone, nameof(phone));
    }

    public void ChangeName(string firstName, string lastName)
    {
        FirstName = Guard.NotEmpty(firstName, nameof(firstName));
        LastName = Guard.NotEmpty(lastName, nameof(lastName));
    }

    public void ChangeLicense(string licenseNumber, string? licenseAuthorityCode, DateTime at)
    {
        CloseCurrent(LicenseHistories, at);
        LicenseNumber = Guard.NotEmpty(licenseNumber, nameof(licenseNumber));
        LicenseAuthorityCode = string.IsNullOrWhiteSpace(licenseAuthorityCode)
            ? null
            : Guard.Code(licenseAuthorityCode, nameof(licenseAuthorityCode));
        LicenseHistories.Add(DoctorLicenseHistory.Create(
            Id,
            NextSequence(LicenseHistories.Select(x => x.SequenceNumber)),
            LicenseNumber,
            LicenseAuthorityCode,
            at));
    }

    public void ChangeSpecialization(string specializationCode, DateTime at)
    {
        var code = Guard.Code(specializationCode, nameof(specializationCode));
        if (CurrentSpecializationCode == code)
            return;

        var currentPrimary = SpecializationCoverages.FirstOrDefault(x => x.IsPrimary && x.To is null);
        currentPrimary?.Close(at);

        CurrentSpecializationCode = code;

        var existing = SpecializationCoverages.FirstOrDefault(x => x.SpecializationCode == code);
        if (existing is not null)
        {
            existing.Reopen(at);
            return;
        }

        SpecializationCoverages.Add(
            DoctorSpecializationCoverage.Create(Id, code, isPrimary: true, from: at));
    }

    public void SetAffiliation(string? institutionName, int? healthInstitutionId, DateTime at)
    {
        var name = EmptyToNull(institutionName);
        var institutionId = healthInstitutionId is <= 0 ? null : healthInstitutionId;

        if (name is null && institutionId is null)
            return;

        CloseCurrent(AffiliationHistories, at);
        CurrentInstitutionName = name ?? CurrentInstitutionName;
        CurrentHealthInstitutionId = institutionId;

        if (CurrentInstitutionName is null)
            return;

        AffiliationHistories.Add(DoctorAffiliationHistory.Create(
            Id,
            NextSequence(AffiliationHistories.Select(x => x.SequenceNumber)),
            CurrentInstitutionName,
            CurrentHealthInstitutionId,
            at));
    }

    public void AddLanguage(string languageCode)
    {
        var coverage = DoctorLanguageCoverage.Create(Id, languageCode);
        if (LanguageCoverages.Any(x => x.LanguageCode == coverage.LanguageCode))
            return;

        LanguageCoverages.Add(coverage);
    }

    public void RemoveLanguage(string languageCode)
    {
        var code = Guard.Code(languageCode, nameof(languageCode));
        var existing = LanguageCoverages.FirstOrDefault(x => x.LanguageCode == code);
        if (existing is not null)
            LanguageCoverages.Remove(existing);
    }

    public void AddDegree(string degreeTypeCode, string? institutionName = null, int? year = null)
    {
        var coverage = DoctorDegreeCoverage.Create(Id, degreeTypeCode, institutionName, year);
        if (DegreeCoverages.Any(x => x.DegreeTypeCode == coverage.DegreeTypeCode))
            return;

        DegreeCoverages.Add(coverage);
    }

    public void RemoveDegree(string degreeTypeCode)
    {
        var code = Guard.Code(degreeTypeCode, nameof(degreeTypeCode));
        var existing = DegreeCoverages.FirstOrDefault(x => x.DegreeTypeCode == code);
        if (existing is not null)
            DegreeCoverages.Remove(existing);
    }

    public void AddWorkingSlot(int dayOfWeek, TimeSpan start, TimeSpan end, DateTime validFrom)
    {
        var sequence = NextSequence(
            WorkingSlots.Where(x => x.DayOfWeek == dayOfWeek).Select(x => x.SequenceNumber));

        WorkingSlots.Add(WorkingSlot.Create(Id, dayOfWeek, sequence, start, end, validFrom));
    }

    public void RemoveWorkingSlot(int dayOfWeek, int sequenceNumber, DateTime at)
    {
        var slot = WorkingSlots.FirstOrDefault(x => x.DayOfWeek == dayOfWeek && x.SequenceNumber == sequenceNumber);
        slot?.Close(at);
    }

    public void AddReview(decimal rating, string? comment, Guid? reviewerUserId, DateTime at)
    {
        Reviews.Add(DoctorReview.Create(
            Id,
            NextSequence(Reviews.Select(x => x.SequenceNumber)),
            rating,
            comment,
            reviewerUserId,
            at));

        TotalReviews = Reviews.Count;
        AverageRating = decimal.Round(Reviews.Average(x => x.Rating), 2);
    }

    private void ChangeStatus(string statusCode, DateTime at)
    {
        if (CurrentStatusCode == DoctorStatusCodes.Deactivated && statusCode != DoctorStatusCodes.Deactivated)
            return;

        if (CurrentStatusCode == statusCode)
            return;

        CloseCurrent(StatusHistories, at);
        CurrentStatusCode = Guard.Code(statusCode, nameof(statusCode));
        StatusHistories.Add(DoctorStatusHistory.Create(
            Id,
            NextSequence(StatusHistories.Select(x => x.SequenceNumber)),
            CurrentStatusCode,
            at));
    }

    private static void CloseCurrent<T>(IEnumerable<T> items, DateTime at)
        where T : class
    {
        foreach (var item in items)
        {
            switch (item)
            {
                case DoctorStatusHistory status when status.To is null:
                    status.Close(at);
                    break;
                case DoctorLicenseHistory license when license.To is null:
                    license.Close(at);
                    break;
                case DoctorAffiliationHistory affiliation when affiliation.To is null:
                    affiliation.Close(at);
                    break;
            }
        }
    }

    private static int NextSequence(IEnumerable<int> values)
    {
        return values.Any() ? values.Max() + 1 : 1;
    }

    private static string? EmptyToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
