using IdentityService.Application.Commands.Authentication;
using IdentityService.Application.Common.Requests;

namespace IdentityService.UnitTests.Application.Validators;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_Passes()
    {
        var result = _validator.Validate(new LoginCommand(new LoginRequest
        {
            Email = "user@neurovision.ai",
            Password = "Secret1"
        }));

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void InvalidEmail_Fails(string email)
    {
        var result = _validator.Validate(new LoginCommand(new LoginRequest
        {
            Email = email,
            Password = "Secret1"
        }));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Email"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    public void InvalidPassword_Fails(string password)
    {
        var result = _validator.Validate(new LoginCommand(new LoginRequest
        {
            Email = "user@neurovision.ai",
            Password = password
        }));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Password"));
    }
}
