namespace IdentityService.UnitTests.Domain;

public class RoleTests
{
    [Fact]
    public void Create_WithValidName_SetsProperties()
    {
        var id = Guid.NewGuid();

        var role = Role.Create(id, RoleNames.Doctor, "Medical professional");

        role.Id.Should().Be(id);
        role.Name.Should().Be(RoleNames.Doctor);
        role.Description.Should().Be("Medical professional");
        role.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        role.UpdatedAtUtc.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyId_GeneratesNewId()
    {
        var role = Role.Create(Guid.Empty, RoleNames.Patient);

        role.Id.Should().NotBe(Guid.Empty);
        role.Description.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_Throws(string? name)
    {
        var act = () => Role.Create(Guid.NewGuid(), name!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_ChangesNameAndDescription_AndSetsUpdatedAt()
    {
        var role = Role.Create(Guid.NewGuid(), RoleNames.Administrator, "Old description");

        role.Update(RoleNames.SuperAdministrator, "Updated description");

        role.Name.Should().Be(RoleNames.SuperAdministrator);
        role.Description.Should().Be("Updated description");
        role.UpdatedAtUtc.Should().NotBeNull();
        role.UpdatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_WithInvalidName_Throws(string? name)
    {
        var role = Role.Create(Guid.NewGuid(), RoleNames.Doctor);

        var act = () => role.Update(name!, "desc");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Restore_PreservesPersistedState()
    {
        var id = Guid.NewGuid();
        var created = DateTime.UtcNow.AddDays(-10);
        var updated = DateTime.UtcNow.AddDays(-1);

        var role = Role.Restore(id, RoleNames.Patient, "Patient user", created, updated);

        role.Id.Should().Be(id);
        role.Name.Should().Be(RoleNames.Patient);
        role.Description.Should().Be("Patient user");
        role.CreatedAtUtc.Should().Be(created);
        role.UpdatedAtUtc.Should().Be(updated);
    }
}
