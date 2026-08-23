using PatientService.Application.Common.Mappings;

namespace PatientService.UnitTests.Application.Mappings;

public class PatientMappingsTests
{
    [Fact]
    public void ToResponse_MapsPatientFieldsAndDateOnly()
    {
        var patient = PatientFactory.Create();

        var response = patient.ToResponse();

        response.Id.Should().Be(patient.Id);
        response.FirstName.Should().Be("Haris");
        response.LastName.Should().Be("Delić");
        response.Email.Should().Be("armanigas78@gmail.com");
        response.DateOfBirth.Should().Be(DateOnly.FromDateTime(patient.DateOfBirth));
        response.GenderCode.Should().Be(GenderCodes.Male);
        response.CurrentStatusCode.Should().Be(PatientStatusCodes.Active);
        response.AssignedDoctorId.Should().Be(PatientFactory.DoctorId);
        response.HeightCm.Should().Be(178);
    }
}
