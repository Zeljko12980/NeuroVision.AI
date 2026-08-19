using LocationService.Domain;

namespace LocationService.UnitTests.Domain;

public class CountryTests
{
    [Fact]
    public void Create_WithValidData_NormalizesCodeAndSetsFields()
    {
        var country = Country.Create("ba", "Bosnia", new DateTime(1995, 12, 14), callingCode: 387, governmentTypeCode: "REP");

        country.Code.Should().Be("BA");
        country.Name.Should().Be("Bosnia");
        country.CallingCode.Should().Be(387);
        country.GovernmentTypeCode.Should().Be("REP");
        country.CapitalSettlementCode.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidCode_Throws(string? code)
    {
        var act = () => Country.Create(code!, "Bosnia", DateTime.UtcNow);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_ChangesMutableFields_AndKeepsExistingFilesWhenNull()
    {
        var country = Country.Create("BA", "Old", new DateTime(1995, 1, 1), flag: [1, 2, 3]);

        country.Update("New", new DateTime(2000, 1, 1), 1, "PARL", 387, null, null, null);

        country.Name.Should().Be("New");
        country.FoundingDate.Should().Be(new DateTime(2000, 1, 1));
        country.CapitalSettlementCode.Should().Be(1);
        country.GovernmentTypeCode.Should().Be("PARL");
        country.Flag.Should().Equal(1, 2, 3);
    }

    [Fact]
    public void SetCapitalSettlement_UpdatesCode()
    {
        var country = Country.Create("BA", "Bosnia", new DateTime(1995, 12, 14));

        country.SetCapitalSettlement(7);

        country.CapitalSettlementCode.Should().Be(7);
    }
}

public class CapitalTests
{
    [Fact]
    public void Create_WithValidPeriod_SetsIdentity()
    {
        var capital = Capital.Create("BA", 1, 1, new DateTime(1995, 12, 14));

        capital.CountryCode.Should().Be("BA");
        capital.SettlementCode.Should().Be(1);
        capital.SequenceNumber.Should().Be(1);
        capital.To.Should().BeNull();
    }

    [Fact]
    public void Create_WhenToIsBeforeFrom_Throws()
    {
        var act = () => Capital.Create("BA", 1, 1, new DateTime(2000, 1, 1), new DateTime(1999, 1, 1));

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithInvalidSequence_Throws()
    {
        var act = () => Capital.Create("BA", 1, 0, DateTime.UtcNow);

        act.Should().Throw<ArgumentException>();
    }
}

public class GovernmentTypeTests
{
    [Fact]
    public void Create_TrimsName()
    {
        var type = GovernmentType.Create("REP", "  Republika  ", "desc");

        type.Code.Should().Be("REP");
        type.Name.Should().Be("Republika");
        type.Description.Should().Be("desc");
    }

    [Fact]
    public void Update_WithEmptyName_Throws()
    {
        var type = GovernmentType.Create("REP", "Republika");

        var act = () => type.Update("  ", null);

        act.Should().Throw<ArgumentException>();
    }
}

public class DateRangeTests
{
    [Fact]
    public void Overlaps_WhenOpenEndedPeriodsIntersect_ReturnsTrue()
    {
        DateRange.Overlaps(
                new DateTime(1990, 1, 1),
                null,
                new DateTime(2000, 1, 1),
                new DateTime(2001, 1, 1))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Overlaps_WhenPeriodsAreAdjacent_ReturnsFalse()
    {
        DateRange.Overlaps(
                new DateTime(1990, 1, 1),
                new DateTime(2000, 1, 1),
                new DateTime(2000, 1, 1),
                null)
            .Should()
            .BeFalse();
    }
}

public class LegalSuccessorTests
{
    [Fact]
    public void Create_WhenCountriesAreTheSame_Throws()
    {
        var act = () => LegalSuccessor.Create("BA", "ba");

        act.Should().Throw<ArgumentException>();
    }
}
