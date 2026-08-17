using IdentityService.Application.Commands.Authentication;

namespace IdentityService.UnitTests.Application.Validators;

public class SetPasswordCommandValidatorTests
{
    private readonly SetPasswordCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_Passes()
    {
        var result = _validator.Validate(new SetPasswordCommand(
            "user@neurovision.ai",
            "token",
            "Secret12"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ShortPassword_Fails()
    {
        var result = _validator.Validate(new SetPasswordCommand(
            "user@neurovision.ai",
            "token",
            "123"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void EmptyToken_Fails()
    {
        var result = _validator.Validate(new SetPasswordCommand(
            "user@neurovision.ai",
            "",
            "Secret12"));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Token");
    }
}
