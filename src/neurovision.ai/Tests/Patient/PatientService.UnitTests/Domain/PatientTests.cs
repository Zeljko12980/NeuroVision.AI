namespace PatientService.UnitTests.Domain;

public class PatientTests
{
    [Fact]
    public void Create_WithValidData_NormalizesEmailAndOpensHistories()
    {
        var patient = PatientFactory.Create(email: "Haris.Delic@NeuroVision.AI");

        patient.Email.Should().Be("haris.delic@neurovision.ai");
        patient.DateOfBirth.Should().Be(new DateTime(1975, 9, 3));
        patient.CurrentStatusCode.Should().Be(PatientStatusCodes.Active);
        patient.AssignedDoctorId.Should().Be(PatientFactory.DoctorId);
        patient.StatusHistories.Should().ContainSingle(item => item.StatusCode == PatientStatusCodes.Active && item.To == null);
        patient.AffiliationHistories.Should().ContainSingle(item => item.To == null);
        patient.DoctorAssignmentHistories.Should().ContainSingle(item => item.DoctorId == PatientFactory.DoctorId);
    }

    [Fact]
    public void Create_WithEmptyId_Throws()
    {
        var act = () => PatientFactory.Create(id: Guid.Empty);

        act.Should().Throw<ArgumentException>().WithParameterName("id");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidFirstName_Throws(string? firstName)
    {
        var act = () => PatientFactory.Create(firstName: firstName!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WhenDateOfBirthIsInTheFuture_Throws()
    {
        var act = () => PatientFactory.Create(dateOfBirth: DateTime.UtcNow.Date.AddDays(1));

        act.Should().Throw<ArgumentException>().WithParameterName("dateOfBirth");
    }

    [Fact]
    public void Create_WhenHeightIsOutOfRange_Throws()
    {
        var act = () => PatientFactory.Create(heightCm: 0);

        act.Should().Throw<ArgumentException>().WithParameterName("heightCm");
    }

    [Fact]
    public void MarkDeceased_PreventsFurtherStatusChanges()
    {
        var patient = PatientFactory.Create();
        var deceasedAt = PatientFactory.CreatedAt.AddDays(10);

        patient.MarkDeceased(deceasedAt);
        patient.Activate(deceasedAt.AddDays(1));

        patient.CurrentStatusCode.Should().Be(PatientStatusCodes.Deceased);
        patient.StatusHistories.Should().HaveCount(2);
        patient.StatusHistories.Should().ContainSingle(item => item.StatusCode == PatientStatusCodes.Deceased && item.To == null);
    }

    [Fact]
    public void AssignDoctor_WhenDifferent_ClosesPreviousAssignment()
    {
        var patient = PatientFactory.Create();
        var nextDoctor = Guid.Parse("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2");
        var at = PatientFactory.CreatedAt.AddDays(3);

        patient.AssignDoctor(nextDoctor, at);

        patient.AssignedDoctorId.Should().Be(nextDoctor);
        patient.DoctorAssignmentHistories.Should().HaveCount(2);
        patient.DoctorAssignmentHistories.Should().ContainSingle(item => item.DoctorId == PatientFactory.DoctorId && item.To == at);
        patient.DoctorAssignmentHistories.Should().ContainSingle(item => item.DoctorId == nextDoctor && item.To == null);
    }

    [Fact]
    public void AssignDoctor_WhenSameDoctor_DoesNotAddHistory()
    {
        var patient = PatientFactory.Create();

        patient.AssignDoctor(PatientFactory.DoctorId, PatientFactory.CreatedAt.AddDays(1));

        patient.DoctorAssignmentHistories.Should().ContainSingle();
    }

    [Fact]
    public void UnassignDoctor_ClearsCurrentAssignment()
    {
        var patient = PatientFactory.Create();
        var at = PatientFactory.CreatedAt.AddDays(2);

        patient.UnassignDoctor(at);

        patient.AssignedDoctorId.Should().BeNull();
        patient.DoctorAssignmentHistories.Should().ContainSingle(item => item.To == at);
    }

    [Fact]
    public void ChangeInsurance_OpensNewHistoryPeriod()
    {
        var patient = PatientFactory.Create();

        patient.ChangeInsurance("fbih", "POL-1", PatientFactory.CreatedAt);
        patient.ChangeInsurance("rs", "POL-2", PatientFactory.CreatedAt.AddDays(1));

        patient.CurrentInsurancePayerCode.Should().Be("RS");
        patient.CurrentInsurancePolicyNumber.Should().Be("POL-2");
        patient.InsuranceHistories.Should().HaveCount(2);
        patient.InsuranceHistories.Should().ContainSingle(item => item.PayerCode == "FBIH" && item.To.HasValue);
    }

    [Fact]
    public void AddLanguage_IgnoresDuplicates()
    {
        var patient = PatientFactory.Create();

        patient.AddLanguage("bs");
        patient.AddLanguage("BS");
        patient.AddLanguage("en");

        patient.LanguageCoverages.Select(item => item.LanguageCode).Should().Equal("BS", "EN");
    }

    [Fact]
    public void RemoveLanguage_RemovesExistingCoverage()
    {
        var patient = PatientFactory.Create();
        patient.AddLanguage("BS");
        patient.AddLanguage("EN");

        patient.RemoveLanguage("bs");

        patient.LanguageCoverages.Should().ContainSingle(item => item.LanguageCode == "EN");
    }

    [Fact]
    public void GrantConsent_ReopensRevokedConsent()
    {
        var patient = PatientFactory.Create();
        var grantedAt = PatientFactory.CreatedAt;
        var revokedAt = grantedAt.AddDays(1);
        var reopenedAt = grantedAt.AddDays(2);

        patient.GrantConsent("data", grantedAt);
        patient.RevokeConsent("DATA", revokedAt);
        patient.GrantConsent("DATA", reopenedAt);

        var consent = patient.ConsentCoverages.Should().ContainSingle(item => item.ConsentTypeCode == "DATA").Subject;
        consent.From.Should().Be(reopenedAt);
        consent.To.Should().BeNull();
    }

    [Fact]
    public void AddEmergencyContact_SequencesContacts()
    {
        var patient = PatientFactory.Create();

        patient.AddEmergencyContact("Aida Delić", "+38762222444", "spou");
        patient.AddEmergencyContact("Amra Delić", "+38761111000", "par");

        patient.EmergencyContacts.Should().HaveCount(2);
        patient.EmergencyContacts.Select(item => item.SequenceNumber).Should().Equal(1, 2);
        patient.EmergencyContacts.Last().RelationshipCode.Should().Be("PAR");
    }

    [Fact]
    public void UpdateContact_NormalizesEmail()
    {
        var patient = PatientFactory.Create();

        patient.UpdateContact("NEW@Email.com", "+38761111222");

        patient.Email.Should().Be("new@email.com");
        patient.Phone.Should().Be("+38761111222");
    }
}

public class CatalogTests
{
    [Fact]
    public void Gender_Create_NormalizesCodeAndTrimsName()
    {
        var gender = Gender.Create("m", "  Male  ");

        gender.Code.Should().Be("M");
        gender.Name.Should().Be("Male");
    }

    [Fact]
    public void PatientStatus_Update_WithEmptyName_Throws()
    {
        var status = PatientStatus.Create("ACT", "Active");

        var act = () => status.Update("  ", null);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void BloodType_Create_UppercasesCode()
    {
        var type = BloodType.Create("ap", "A+");

        type.Code.Should().Be("AP");
        type.Name.Should().Be("A+");
    }
}

public class GuardTests
{
    [Fact]
    public void Code_WhenLongerThanMaxLength_Throws()
    {
        var act = () => Guard.Code("TOOLONGCODE", "code");

        act.Should().Throw<ArgumentException>().WithParameterName("code");
    }

    [Fact]
    public void DateRange_WhenToIsBeforeFrom_Throws()
    {
        var act = () => DateRange.EnsureValid(new DateTime(2024, 2, 1), new DateTime(2024, 1, 1));

        act.Should().Throw<ArgumentException>();
    }
}

public class PatientEmergencyContactTests
{
    [Fact]
    public void Create_WhenSequenceIsInvalid_Throws()
    {
        var act = () => PatientEmergencyContact.Create(
            PatientFactory.DefaultId,
            0,
            "Aida Delić",
            "+38762222444",
            "SPOU");

        act.Should().Throw<ArgumentException>().WithParameterName("sequenceNumber");
    }
}

public class PatientConsentCoverageTests
{
    [Fact]
    public void Create_WhenToIsBeforeFrom_Throws()
    {
        var act = () => PatientConsentCoverage.Create(
            PatientFactory.DefaultId,
            "DATA",
            new DateTime(2024, 2, 1),
            new DateTime(2024, 1, 1));

        act.Should().Throw<ArgumentException>();
    }
}
