using IdentityService.Application.Commands.Authentication;

namespace IdentityService.UnitTests.Application.Validators;

public class ChangePasswordCommandValidatorTests
{
    private readonly ChangePasswordCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_Passes()
    {
        var result = _validator.Validate(new ChangePasswordCommand("OldPass12", "NewPass12"));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ShortNewPassword_Fails()
    {
        var result = _validator.Validate(new ChangePasswordCommand("OldPass12", "short"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NewPassword");
    }

    [Fact]
    public void SamePasswords_Fail()
    {
        var result = _validator.Validate(new ChangePasswordCommand("SamePass1", "SamePass1"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NewPassword");
    }
}
