using IdentityService.Application.Commands.Role;

namespace IdentityService.UnitTests.Application.Validators;

public class CreateRoleCommandValidatorTests
{
    private readonly CreateRoleCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_Passes()
    {
        var result = _validator.Validate(new CreateRoleCommand("Radiologist", "Reads scans"));

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("Bad Role")]
    [InlineData(" Doctor")]
    [InlineData("Doctor ")]
    public void InvalidRoleName_Fails(string roleName)
    {
        var result = _validator.Validate(new CreateRoleCommand(roleName, null));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RoleName");
    }

    [Fact]
    public void DescriptionLongerThan250_Fails()
    {
        var result = _validator.Validate(new CreateRoleCommand("Doctor", new string('x', 251)));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }
}
