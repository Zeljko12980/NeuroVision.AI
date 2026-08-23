using DoctorService.Application.Common.Mappings;

namespace DoctorService.UnitTests.Application.Mappings;

public class DoctorMappingsTests
{
    [Fact]
    public void ToResponse_MapsDoctorFields()
    {
        var doctor = DoctorFactory.Create();

        var response = doctor.ToResponse();

        response.Id.Should().Be(doctor.Id);
        response.FirstName.Should().Be("Željko");
        response.LastName.Should().Be("Ikanović");
        response.Email.Should().Be("ikanoviczeljko362@gmail.com");
        response.LicenseNumber.Should().Be("LIC-1001");
        response.CurrentSpecializationCode.Should().Be("NEURO");
        response.CurrentStatusCode.Should().Be(DoctorStatusCodes.Active);
        response.IsAvailable.Should().BeTrue();
        response.AverageRating.Should().Be(0);
    }

    [Fact]
    public void ToResponse_MapsSpecializationFields()
    {
        var specialization = Specialization.Create("neuro", "Neurology", "Brain");

        var response = specialization.ToResponse();

        response.Code.Should().Be("NEURO");
        response.Name.Should().Be("Neurology");
        response.Description.Should().Be("Brain");
    }
}
