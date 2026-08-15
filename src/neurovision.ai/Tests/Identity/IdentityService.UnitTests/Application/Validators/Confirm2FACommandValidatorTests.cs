using IdentityService.Application.Commands.Authentication;
using IdentityService.Application.Common.Requests;

namespace IdentityService.UnitTests.Application.Validators;

public class Confirm2FACommandValidatorTests
{
    private readonly Confirm2FACommandValidator _validator = new();

    [Fact]
    public void ValidCommand_Passes()
    {
        var result = _validator.Validate(new Confirm2FACommand(new Confirm2FARequest
        {
            Email = "user@neurovision.ai",
            Code = "123456"
        }));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CodeNotSixCharacters_Fails()
    {
        var result = _validator.Validate(new Confirm2FACommand(new Confirm2FARequest
        {
            Email = "user@neurovision.ai",
            Code = "123"
        }));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Code"));
    }
}
