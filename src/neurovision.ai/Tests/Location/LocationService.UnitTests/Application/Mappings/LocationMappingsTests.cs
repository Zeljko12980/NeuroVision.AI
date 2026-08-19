using LocationService.Application.Common.Mappings;
using LocationService.Application.Feature.Country.Command.Create;

namespace LocationService.UnitTests.Application.Mappings;

public class LocationMappingsTests
{
    [Fact]
    public void ToResponse_MapsCountryFields()
    {
        var country = Country.Create("BA", "Bosnia", new DateTime(1995, 12, 14), callingCode: 387);

        var response = country.ToResponse();

        response.Code.Should().Be("BA");
        response.Name.Should().Be("Bosnia");
        response.CallingCode.Should().Be(387);
        response.SettlementCount.Should().Be(0);
    }
}
