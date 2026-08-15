using IdentityService.Application.Common.Mappings;

namespace IdentityService.UnitTests.Application.Mappings;

public class IdentityMappingsTests
{
    [Fact]
    public void UserToResponse_MapsIdentityFields()
    {
        var user = User.Create(Guid.NewGuid(), "doctor.jane", "jane@neurovision.ai");

        var response = user.ToResponse();

        response.Id.Should().Be(user.Id);
        response.UserName.Should().Be(user.UserName);
        response.Email.Should().Be(user.Email);
    }

    [Fact]
    public void RoleToResponse_MapsNameAndDescription()
    {
        var role = Role.Create(Guid.NewGuid(), RoleNames.Patient, "Patient user");

        var response = role.ToResponse();

        response.Id.Should().Be(role.Id);
        response.Name.Should().Be(RoleNames.Patient);
        response.Description.Should().Be("Patient user");
        response.UserCount.Should().BeNull();
        response.Status.Should().BeNull();
    }
}
