using IdentityService.Application.Commands.Authentication;

namespace IdentityService.UnitTests.Application.Validators;

public class UpdateProfileCommandValidatorTests
{
    private readonly UpdateProfileCommandValidator _validator = new();

    [Theory]
    [InlineData("+387 61 111 222")]
    [InlineData("+38761111222")]
    [InlineData("+385 91 123 4567")]
    [InlineData("+1 202 555 0123")]
    [InlineData("+44 20 7946 0958")]
    public void ValidInternationalPhone_Passes(string phone)
    {
        var result = _validator.Validate(new UpdateProfileCommand("doctor.jane", phone));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void EmptyUserName_Fails()
    {
        var result = _validator.Validate(new UpdateProfileCommand(" ", "+38761111222"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserName");
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("061111222")]
    [InlineData("38761111222")]
    [InlineData("+012345678")]
    [InlineData("+3876")]
    public void InvalidPhoneNumber_Fails(string phone)
    {
        var result = _validator.Validate(new UpdateProfileCommand("doctor.jane", phone));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PhoneNumber");
    }

    [Fact]
    public void EmptyPhoneNumber_Passes()
    {
        var result = _validator.Validate(new UpdateProfileCommand("doctor.jane", null));
        result.IsValid.Should().BeTrue();
    }
}
