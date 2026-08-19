using LocationService.Application.Common.Request;
using LocationService.Application.Feature.Country.Command.Create;
using LocationService.Application.Feature.Country.Command.Delete;

namespace LocationService.UnitTests.Application.Validators;

public class CountryValidatorTests
{
    [Fact]
    public void CreateValidator_WhenCodeMissing_Fails()
    {
        var validator = new CreateCountryCommandValidator();

        var result = validator.Validate(new CreateCountryCommand(new CreateCountryRequest
        {
            Code = "",
            Name = "Bosnia",
            FoundingDate = DateTime.UtcNow
        }));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName.Contains("Code"));
    }

    [Fact]
    public void CreateValidator_WhenRequestIsValid_Passes()
    {
        var validator = new CreateCountryCommandValidator();

        var result = validator.Validate(new CreateCountryCommand(new CreateCountryRequest
        {
            Code = "BA",
            Name = "Bosnia",
            FoundingDate = new DateTime(1995, 12, 14)
        }));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DeleteValidator_WhenCodeEmpty_Fails()
    {
        var validator = new DeleteCountryCommandValidator();

        var result = validator.Validate(new DeleteCountryCommand(""));

        result.IsValid.Should().BeFalse();
    }
}
