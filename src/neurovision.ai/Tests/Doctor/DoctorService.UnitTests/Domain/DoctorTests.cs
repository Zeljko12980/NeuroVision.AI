namespace DoctorService.UnitTests.Domain;

public class DoctorTests
{
    [Fact]
    public void Create_WithValidData_NormalizesEmailAndOpensHistories()
    {
        var doctor = DoctorFactory.Create(email: "Zeljko.Ikanovic@NeuroVision.AI");

        doctor.Email.Should().Be("zeljko.ikanovic@neurovision.ai");
        doctor.CurrentStatusCode.Should().Be(DoctorStatusCodes.Active);
        doctor.CurrentSpecializationCode.Should().Be("NEURO");
        doctor.LicenseNumber.Should().Be("LIC-1001");
        doctor.IsAvailable.Should().BeTrue();
        doctor.AverageRating.Should().Be(0);
        doctor.StatusHistories.Should().ContainSingle(item => item.StatusCode == DoctorStatusCodes.Active && item.To == null);
        doctor.LicenseHistories.Should().ContainSingle(item => item.LicenseNumber == "LIC-1001" && item.To == null);
        doctor.SpecializationCoverages.Should().ContainSingle(item => item.IsPrimary && item.To == null);
        doctor.AffiliationHistories.Should().ContainSingle(item => item.To == null);
    }

    [Fact]
    public void Create_WithEmptyId_Throws()
    {
        var act = () => DoctorFactory.Create(id: Guid.Empty);

        act.Should().Throw<ArgumentException>().WithParameterName("id");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidFirstName_Throws(string? firstName)
    {
        var act = () => DoctorFactory.Create(firstName: firstName!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deactivate_PreventsFurtherStatusChanges()
    {
        var doctor = DoctorFactory.Create();
        var at = DoctorFactory.CreatedAt.AddDays(10);

        doctor.Deactivate(at);
        doctor.Activate(at.AddDays(1));

        doctor.CurrentStatusCode.Should().Be(DoctorStatusCodes.Deactivated);
        doctor.StatusHistories.Should().HaveCount(2);
        doctor.StatusHistories.Should().ContainSingle(item =>
            item.StatusCode == DoctorStatusCodes.Deactivated && item.To == null);
    }

    [Fact]
    public void Suspend_ClosesPreviousStatus()
    {
        var doctor = DoctorFactory.Create();
        var at = DoctorFactory.CreatedAt.AddDays(2);

        doctor.Suspend(at);

        doctor.CurrentStatusCode.Should().Be(DoctorStatusCodes.Suspended);
        doctor.StatusHistories.Should().ContainSingle(item =>
            item.StatusCode == DoctorStatusCodes.Active && item.To == at);
    }

    [Fact]
    public void ChangeLicense_OpensNewHistoryPeriod()
    {
        var doctor = DoctorFactory.Create();
        var at = DoctorFactory.CreatedAt.AddDays(1);

        doctor.ChangeLicense("LIC-2002", "ljek", at);

        doctor.LicenseNumber.Should().Be("LIC-2002");
        doctor.LicenseAuthorityCode.Should().Be("LJEK");
        doctor.LicenseHistories.Should().HaveCount(2);
        doctor.LicenseHistories.Should().ContainSingle(item => item.LicenseNumber == "LIC-1001" && item.To == at);
        doctor.LicenseHistories.Should().ContainSingle(item => item.LicenseNumber == "LIC-2002" && item.To == null);
    }

    [Fact]
    public void ChangeSpecialization_WhenDifferent_ClosesPrimaryAndAddsCoverage()
    {
        var doctor = DoctorFactory.Create();
        var at = DoctorFactory.CreatedAt.AddDays(3);

        doctor.ChangeSpecialization("cardio", at);

        doctor.CurrentSpecializationCode.Should().Be("CARDIO");
        doctor.SpecializationCoverages.Should().HaveCount(2);
        doctor.SpecializationCoverages.Should().ContainSingle(item =>
            item.SpecializationCode == "NEURO" && item.To == at && !item.IsPrimary);
        doctor.SpecializationCoverages.Should().ContainSingle(item =>
            item.SpecializationCode == "CARDIO" && item.IsPrimary && item.To == null);
    }

    [Fact]
    public void ChangeSpecialization_WhenSame_DoesNothing()
    {
        var doctor = DoctorFactory.Create();

        doctor.ChangeSpecialization("NEURO", DoctorFactory.CreatedAt.AddDays(1));

        doctor.SpecializationCoverages.Should().ContainSingle();
    }

    [Fact]
    public void AddLanguage_IgnoresDuplicates()
    {
        var doctor = DoctorFactory.Create();

        doctor.AddLanguage("bs");
        doctor.AddLanguage("BS");
        doctor.AddLanguage("en");

        doctor.LanguageCoverages.Select(item => item.LanguageCode).Should().Equal("BS", "EN");
    }

    [Fact]
    public void AddDegree_IgnoresDuplicates()
    {
        var doctor = DoctorFactory.Create();

        doctor.AddDegree("md", "UNSA", 2010);
        doctor.AddDegree("MD");

        doctor.DegreeCoverages.Should().ContainSingle(item => item.DegreeTypeCode == "MD");
    }

    [Fact]
    public void AddWorkingSlot_ThenRemove_ClosesSlot()
    {
        var doctor = DoctorFactory.Create();
        var from = DoctorFactory.CreatedAt;
        var closeAt = from.AddDays(7);

        doctor.AddWorkingSlot(1, TimeSpan.FromHours(8), TimeSpan.FromHours(16), from);
        doctor.RemoveWorkingSlot(1, 1, closeAt);

        doctor.WorkingSlots.Should().ContainSingle(item => item.DayOfWeek == 1 && item.ValidTo == closeAt);
    }

    [Fact]
    public void AddReview_UpdatesAverageRating()
    {
        var doctor = DoctorFactory.Create();

        doctor.AddReview(5, "Excellent", Guid.NewGuid(), DoctorFactory.CreatedAt);
        doctor.AddReview(4, "Good", Guid.NewGuid(), DoctorFactory.CreatedAt.AddDays(1));

        doctor.TotalReviews.Should().Be(2);
        doctor.AverageRating.Should().Be(4.5m);
    }

    [Fact]
    public void SetAvailability_UpdatesFlagAndLastActive()
    {
        var doctor = DoctorFactory.Create();
        var at = DoctorFactory.CreatedAt.AddHours(3);

        doctor.SetAvailability(false, at);

        doctor.IsAvailable.Should().BeFalse();
        doctor.LastActive.Should().Be(at);
    }

    [Fact]
    public void UpdateContact_NormalizesEmail()
    {
        var doctor = DoctorFactory.Create();

        doctor.UpdateContact("NEW@Email.com", "+38762222333");

        doctor.Email.Should().Be("new@email.com");
        doctor.Phone.Should().Be("+38762222333");
    }
}

public class CatalogTests
{
    [Fact]
    public void Specialization_Create_NormalizesCodeAndTrimsName()
    {
        var specialization = Specialization.Create("neuro", "  Neurology  ");

        specialization.Code.Should().Be("NEURO");
        specialization.Name.Should().Be("Neurology");
    }

    [Fact]
    public void DoctorStatus_Update_WithEmptyName_Throws()
    {
        var status = DoctorStatus.Create("ACT", "Active");

        var act = () => status.Update("  ", null);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LicenseAuthority_Create_UppercasesCode()
    {
        var authority = LicenseAuthority.Create("kzk", "Komora");

        authority.Code.Should().Be("KZK");
        authority.Name.Should().Be("Komora");
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

public class WorkingSlotTests
{
    [Fact]
    public void Create_WhenEndIsNotAfterStart_Throws()
    {
        var act = () => WorkingSlot.Create(
            DoctorFactory.DefaultId,
            1,
            1,
            TimeSpan.FromHours(10),
            TimeSpan.FromHours(10),
            DoctorFactory.CreatedAt);

        act.Should().Throw<ArgumentException>().WithParameterName("end");
    }

    [Fact]
    public void Create_WhenDayOfWeekIsInvalid_Throws()
    {
        var act = () => WorkingSlot.Create(
            DoctorFactory.DefaultId,
            7,
            1,
            TimeSpan.FromHours(8),
            TimeSpan.FromHours(16),
            DoctorFactory.CreatedAt);

        act.Should().Throw<ArgumentException>().WithParameterName("dayOfWeek");
    }
}

public class DoctorReviewTests
{
    [Fact]
    public void Create_WhenRatingIsOutOfRange_Throws()
    {
        var act = () => DoctorReview.Create(
            DoctorFactory.DefaultId,
            1,
            6,
            "Too high",
            null,
            DoctorFactory.CreatedAt);

        act.Should().Throw<ArgumentException>().WithParameterName("rating");
    }
}
