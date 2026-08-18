using IdentityService.Application.Commands.Authentication;

namespace IdentityService.UnitTests.Application.Validators;

public class ForgotPasswordCommandValidatorTests
{
    private readonly ForgotPasswordCommandValidator _validator = new();

    [Fact]
    public void ValidEmail_Passes()
    {
        var result = _validator.Validate(new ForgotPasswordCommand("user@neurovision.ai"));
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void InvalidEmail_Fails(string email)
    {
        var result = _validator.Validate(new ForgotPasswordCommand(email));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }
}
