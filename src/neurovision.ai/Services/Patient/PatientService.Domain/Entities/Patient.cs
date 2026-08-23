namespace PatientService.Domain.Entities;

public class Patient
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public DateTime DateOfBirth { get; private set; }
    public string GenderCode { get; private set; } = null!;
    public string? BloodTypeCode { get; private set; }
    public string? NationalId { get; private set; }
    public string CurrentStatusCode { get; private set; } = null!;
    public string? ProfilePictureUrl { get; private set; }
    public string? Notes { get; private set; }
    public int? CurrentHealthInstitutionId { get; private set; }
    public string? CurrentInstitutionName { get; private set; }
    public Guid? AssignedDoctorId { get; private set; }
    public string? CurrentInsurancePayerCode { get; private set; }
    public string? CurrentInsurancePolicyNumber { get; private set; }
    public string? AddressLine { get; private set; }
    public int? SettlementId { get; private set; }
    public int? MunicipalityId { get; private set; }
    public int? CountryId { get; private set; }
    public decimal? HeightCm { get; private set; }
    public decimal? WeightKg { get; private set; }
    public DateTime LastActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public PatientStatus Status { get; private set; } = null!;
    public Gender Gender { get; private set; } = null!;
    public BloodType? BloodType { get; private set; }
    public InsurancePayer? CurrentInsurancePayer { get; private set; }

    public ICollection<PatientStatusHistory> StatusHistories { get; private set; } = new List<PatientStatusHistory>();
    public ICollection<PatientAffiliationHistory> AffiliationHistories { get; private set; } = new List<PatientAffiliationHistory>();
    public ICollection<PatientInsuranceHistory> InsuranceHistories { get; private set; } = new List<PatientInsuranceHistory>();
    public ICollection<PatientDoctorAssignmentHistory> DoctorAssignmentHistories { get; private set; } = new List<PatientDoctorAssignmentHistory>();
    public ICollection<PatientLanguageCoverage> LanguageCoverages { get; private set; } = new List<PatientLanguageCoverage>();
    public ICollection<PatientAllergyCoverage> AllergyCoverages { get; private set; } = new List<PatientAllergyCoverage>();
    public ICollection<PatientConditionCoverage> ConditionCoverages { get; private set; } = new List<PatientConditionCoverage>();
    public ICollection<PatientConsentCoverage> ConsentCoverages { get; private set; } = new List<PatientConsentCoverage>();
    public ICollection<PatientEmergencyContact> EmergencyContacts { get; private set; } = new List<PatientEmergencyContact>();

    private Patient()
    {
    }

    public static Patient Create(
        Guid id,
        string firstName,
        string lastName,
        string email,
        string phone,
        DateTime dateOfBirth,
        string genderCode,
        string statusCode,
        string? bloodTypeCode,
        string? nationalId,
        string? profilePictureUrl,
        string? notes,
        int? healthInstitutionId,
        string? institutionName,
        Guid? assignedDoctorId,
        string? addressLine,
        int? settlementId,
        int? municipalityId,
        int? countryId,
        decimal? heightCm,
        decimal? weightKg,
        DateTime createdAt)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Patient id is required.", nameof(id));

        EnsureDateOfBirth(dateOfBirth, createdAt);
        EnsureBodyMetrics(heightCm, weightKg);

        var patient = new Patient
        {
            Id = id,
            FirstName = Guard.NotEmpty(firstName, nameof(firstName)),
            LastName = Guard.NotEmpty(lastName, nameof(lastName)),
            Email = Guard.NotEmpty(email, nameof(email)).ToLowerInvariant(),
            Phone = Guard.NotEmpty(phone, nameof(phone)),
            DateOfBirth = dateOfBirth.Date,
            GenderCode = Guard.Code(genderCode, nameof(genderCode)),
            BloodTypeCode = EmptyToNullCode(bloodTypeCode),
            NationalId = EmptyToNull(nationalId),
            CurrentStatusCode = Guard.Code(statusCode, nameof(statusCode)),
            ProfilePictureUrl = EmptyToNull(profilePictureUrl),
            Notes = EmptyToNull(notes),
            CurrentHealthInstitutionId = healthInstitutionId is <= 0 ? null : healthInstitutionId,
            CurrentInstitutionName = EmptyToNull(institutionName),
            AssignedDoctorId = assignedDoctorId == Guid.Empty ? null : assignedDoctorId,
            AddressLine = EmptyToNull(addressLine),
            SettlementId = settlementId is <= 0 ? null : settlementId,
            MunicipalityId = municipalityId is <= 0 ? null : municipalityId,
            CountryId = countryId is <= 0 ? null : countryId,
            HeightCm = heightCm,
            WeightKg = weightKg,
            LastActive = createdAt,
            CreatedAt = createdAt
        };

        patient.StatusHistories.Add(PatientStatusHistory.Create(id, 1, patient.CurrentStatusCode, createdAt));

        if (!string.IsNullOrWhiteSpace(patient.CurrentInstitutionName))
        {
            patient.AffiliationHistories.Add(PatientAffiliationHistory.Create(
                id,
                1,
                patient.CurrentInstitutionName,
                patient.CurrentHealthInstitutionId,
                createdAt));
        }

        if (patient.AssignedDoctorId is Guid doctorId)
        {
            patient.DoctorAssignmentHistories.Add(
                PatientDoctorAssignmentHistory.Create(id, 1, doctorId, createdAt));
        }

        return patient;
    }

    public void Activate(DateTime at) => ChangeStatus(PatientStatusCodes.Active, at);

    public void Deactivate(DateTime at) => ChangeStatus(PatientStatusCodes.Inactive, at);

    public void Archive(DateTime at) => ChangeStatus(PatientStatusCodes.Archived, at);

    public void MarkDeceased(DateTime at) => ChangeStatus(PatientStatusCodes.Deceased, at);

    public void UpdateProfile(string? pictureUrl, string? notes)
    {
        if (!string.IsNullOrWhiteSpace(pictureUrl))
            ProfilePictureUrl = pictureUrl.Trim();

        Notes = EmptyToNull(notes) ?? Notes;
    }

    public void Touch(DateTime at) => LastActive = at;

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

    public void UpdateDemographics(
        DateTime dateOfBirth,
        string genderCode,
        string? bloodTypeCode,
        string? nationalId,
        decimal? heightCm,
        decimal? weightKg)
    {
        EnsureDateOfBirth(dateOfBirth, DateTime.UtcNow);
        EnsureBodyMetrics(heightCm, weightKg);

        DateOfBirth = dateOfBirth.Date;
        GenderCode = Guard.Code(genderCode, nameof(genderCode));
        BloodTypeCode = EmptyToNullCode(bloodTypeCode);
        NationalId = EmptyToNull(nationalId);
        HeightCm = heightCm;
        WeightKg = weightKg;
    }

    public void UpdateAddress(string? addressLine, int? settlementId, int? municipalityId, int? countryId)
    {
        AddressLine = EmptyToNull(addressLine);
        SettlementId = settlementId is <= 0 ? null : settlementId;
        MunicipalityId = municipalityId is <= 0 ? null : municipalityId;
        CountryId = countryId is <= 0 ? null : countryId;
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

        AffiliationHistories.Add(PatientAffiliationHistory.Create(
            Id,
            NextSequence(AffiliationHistories.Select(x => x.SequenceNumber)),
            CurrentInstitutionName,
            CurrentHealthInstitutionId,
            at));
    }

    public void AssignDoctor(Guid doctorId, DateTime at)
    {
        if (doctorId == Guid.Empty)
            throw new ArgumentException("Doctor id is required.", nameof(doctorId));

        if (AssignedDoctorId == doctorId)
            return;

        CloseCurrent(DoctorAssignmentHistories, at);
        AssignedDoctorId = doctorId;
        DoctorAssignmentHistories.Add(PatientDoctorAssignmentHistory.Create(
            Id,
            NextSequence(DoctorAssignmentHistories.Select(x => x.SequenceNumber)),
            doctorId,
            at));
    }

    public void UnassignDoctor(DateTime at)
    {
        if (AssignedDoctorId is null)
            return;

        CloseCurrent(DoctorAssignmentHistories, at);
        AssignedDoctorId = null;
    }

    public void ChangeInsurance(string payerCode, string policyNumber, DateTime at)
    {
        CloseCurrent(InsuranceHistories, at);
        CurrentInsurancePayerCode = Guard.Code(payerCode, nameof(payerCode));
        CurrentInsurancePolicyNumber = Guard.NotEmpty(policyNumber, nameof(policyNumber));
        InsuranceHistories.Add(PatientInsuranceHistory.Create(
            Id,
            NextSequence(InsuranceHistories.Select(x => x.SequenceNumber)),
            CurrentInsurancePayerCode,
            CurrentInsurancePolicyNumber,
            at));
    }

    public void AddLanguage(string languageCode)
    {
        var coverage = PatientLanguageCoverage.Create(Id, languageCode);
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

    public void AddAllergy(string allergyCode, string? note = null)
    {
        var coverage = PatientAllergyCoverage.Create(Id, allergyCode, note);
        if (AllergyCoverages.Any(x => x.AllergyCode == coverage.AllergyCode))
            return;

        AllergyCoverages.Add(coverage);
    }

    public void RemoveAllergy(string allergyCode)
    {
        var code = Guard.Code(allergyCode, nameof(allergyCode));
        var existing = AllergyCoverages.FirstOrDefault(x => x.AllergyCode == code);
        if (existing is not null)
            AllergyCoverages.Remove(existing);
    }

    public void AddCondition(string conditionCode, int? diagnosedYear = null, string? note = null)
    {
        var coverage = PatientConditionCoverage.Create(Id, conditionCode, diagnosedYear, note);
        if (ConditionCoverages.Any(x => x.ConditionCode == coverage.ConditionCode))
            return;

        ConditionCoverages.Add(coverage);
    }

    public void RemoveCondition(string conditionCode)
    {
        var code = Guard.Code(conditionCode, nameof(conditionCode));
        var existing = ConditionCoverages.FirstOrDefault(x => x.ConditionCode == code);
        if (existing is not null)
            ConditionCoverages.Remove(existing);
    }

    public void GrantConsent(string consentTypeCode, DateTime at)
    {
        var code = Guard.Code(consentTypeCode, nameof(consentTypeCode));
        var existing = ConsentCoverages.FirstOrDefault(x => x.ConsentTypeCode == code);
        if (existing is not null)
        {
            if (existing.To is null)
                return;

            existing.Reopen(at);
            return;
        }

        ConsentCoverages.Add(PatientConsentCoverage.Create(Id, code, at));
    }

    public void RevokeConsent(string consentTypeCode, DateTime at)
    {
        var code = Guard.Code(consentTypeCode, nameof(consentTypeCode));
        var existing = ConsentCoverages.FirstOrDefault(x => x.ConsentTypeCode == code && x.To is null);
        existing?.Revoke(at);
    }

    public void AddEmergencyContact(string fullName, string phone, string relationshipCode)
    {
        EmergencyContacts.Add(PatientEmergencyContact.Create(
            Id,
            NextSequence(EmergencyContacts.Select(x => x.SequenceNumber)),
            fullName,
            phone,
            relationshipCode));
    }

    public void RemoveEmergencyContact(int sequenceNumber)
    {
        var existing = EmergencyContacts.FirstOrDefault(x => x.SequenceNumber == sequenceNumber);
        if (existing is not null)
            EmergencyContacts.Remove(existing);
    }

    private void ChangeStatus(string statusCode, DateTime at)
    {
        if (CurrentStatusCode == PatientStatusCodes.Deceased && statusCode != PatientStatusCodes.Deceased)
            return;

        if (CurrentStatusCode == statusCode)
            return;

        CloseCurrent(StatusHistories, at);
        CurrentStatusCode = Guard.Code(statusCode, nameof(statusCode));
        StatusHistories.Add(PatientStatusHistory.Create(
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
                case PatientStatusHistory status when status.To is null:
                    status.Close(at);
                    break;
                case PatientAffiliationHistory affiliation when affiliation.To is null:
                    affiliation.Close(at);
                    break;
                case PatientInsuranceHistory insurance when insurance.To is null:
                    insurance.Close(at);
                    break;
                case PatientDoctorAssignmentHistory assignment when assignment.To is null:
                    assignment.Close(at);
                    break;
            }
        }
    }

    private static int NextSequence(IEnumerable<int> values)
    {
        return values.Any() ? values.Max() + 1 : 1;
    }

    private static void EnsureDateOfBirth(DateTime dateOfBirth, DateTime now)
    {
        if (dateOfBirth.Date > now.Date)
            throw new ArgumentException("Date of birth cannot be in the future.", nameof(dateOfBirth));

        if (dateOfBirth.Year < 1900)
            throw new ArgumentException("Date of birth is out of range.", nameof(dateOfBirth));
    }

    private static void EnsureBodyMetrics(decimal? heightCm, decimal? weightKg)
    {
        if (heightCm is <= 0 or > 300)
            throw new ArgumentException("Height is out of range.", nameof(heightCm));

        if (weightKg is <= 0 or > 500)
            throw new ArgumentException("Weight is out of range.", nameof(weightKg));
    }

    private static string? EmptyToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string? EmptyToNullCode(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : Guard.Code(value, "code");
    }
}
