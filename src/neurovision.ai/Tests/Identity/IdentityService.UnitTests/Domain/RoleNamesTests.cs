namespace IdentityService.UnitTests.Domain;

public class RoleNamesTests
{
    [Fact]
    public void Definitions_ContainAllSeedRoles()
    {
        RoleNames.Definitions.Select(d => d.Name).Should().Equal(
            RoleNames.SuperAdministrator,
            RoleNames.Administrator,
            RoleNames.Doctor,
            RoleNames.Patient);
    }

    [Fact]
    public void Constants_HaveExpectedValues()
    {
        RoleNames.SuperAdministrator.Should().Be("SuperAdministrator");
        RoleNames.Administrator.Should().Be("Administrator");
        RoleNames.Doctor.Should().Be("Doctor");
        RoleNames.Patient.Should().Be("Patient");
    }

    [Fact]
    public void AuthPolicies_MatchConfiguredPolicyNames()
    {
        AuthPolicies.SuperAdmin.Should().Be("SuperAdminPolicy");
        AuthPolicies.Doctor.Should().Be("DoctorPolicy");
        AuthPolicies.Patient.Should().Be("PatientPolicy");
    }
}
