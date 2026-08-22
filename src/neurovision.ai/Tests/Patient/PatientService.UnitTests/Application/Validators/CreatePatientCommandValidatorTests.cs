using PatientService.Application.Common.Request;
using PatientService.Application.Feature.Patient.Command.Create;

namespace PatientService.UnitTests.Application.Validators;

public class CreatePatientCommandValidatorTests
{
    private readonly CreatePatientCommandValidator _validator = new();

    [Fact]
    public void WhenRequestIsValid_Passes()
    {
        var result = _validator.Validate(new CreatePatientCommand(ValidRequest()));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void WhenEmailInvalid_Fails()
    {
        var request = ValidRequest();
        request.Email = "not-an-email";

        var result = _validator.Validate(new CreatePatientCommand(request));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName.Contains("Email"));
    }

    [Fact]
    public void WhenPhoneInvalid_Fails()
    {
        var request = ValidRequest();
        request.PhoneNumber = "abc";

        var result = _validator.Validate(new CreatePatientCommand(request));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName.Contains("PhoneNumber"));
    }

    [Fact]
    public void WhenDateOfBirthInFuture_Fails()
    {
        var request = ValidRequest();
        request.DateOfBirth = DateTime.UtcNow.Date.AddDays(1);

        var result = _validator.Validate(new CreatePatientCommand(request));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName.Contains("DateOfBirth"));
    }

    [Fact]
    public void WhenFirstNameMissing_Fails()
    {
        var request = ValidRequest();
        request.FirstName = "";

        var result = _validator.Validate(new CreatePatientCommand(request));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName.Contains("FirstName"));
    }

    private static CreatePatientRequest ValidRequest() =>
        new()
        {
            FirstName = "Haris",
            LastName = "Delić",
            Email = "armanigas78@gmail.com",
            PhoneNumber = "+38762222333",
            DateOfBirth = new DateTime(1975, 9, 3),
            Gender = "M",
            Languages = "BS,EN"
        };
}
