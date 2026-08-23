using DoctorService.Application.Common.Request;
using DoctorService.Application.Feature.Doctor.Command.Create;

namespace DoctorService.UnitTests.Application.Validators;

public class CreateDoctorCommandValidatorTests
{
    private readonly CreateDoctorCommandValidator _validator = new();

    [Fact]
    public void WhenRequestIsValid_Passes()
    {
        var result = _validator.Validate(new CreateDoctorCommand(ValidRequest()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void WhenEmailInvalid_Fails()
    {
        var request = ValidRequest();
        request.Email = "not-an-email";

        var result = _validator.Validate(new CreateDoctorCommand(request));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName.Contains("Email"));
    }

    [Fact]
    public void WhenPhoneInvalid_Fails()
    {
        var request = ValidRequest();
        request.PhoneNumber = "abc";

        var result = _validator.Validate(new CreateDoctorCommand(request));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName.Contains("PhoneNumber"));
    }

    [Fact]
    public void WhenLicenseNumberMissing_Fails()
    {
        var request = ValidRequest();
        request.LicenseNumber = "";

        var result = _validator.Validate(new CreateDoctorCommand(request));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName.Contains("LicenseNumber"));
    }

    [Fact]
    public void WhenSpecializationMissing_Fails()
    {
        var request = ValidRequest();
        request.Specialization = "";

        var result = _validator.Validate(new CreateDoctorCommand(request));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName.Contains("Specialization"));
    }

    private static CreateDoctorRequest ValidRequest() =>
        new()
        {
            FirstName = "Željko",
            LastName = "Ikanović",
            LicenseNumber = "LIC-1001",
            LicenseAuthorityCode = "KZK",
            Specialization = "NEURO",
            Email = "ikanoviczeljko362@gmail.com",
            PhoneNumber = "+38761111222",
            Languages = "BS,EN"
        };
}
