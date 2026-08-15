namespace IdentityService.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Create_WithValidData_SetsIdentityAndEnablesTwoFactor()
    {
        var id = Guid.NewGuid();

        var user = User.Create(id, "doctor.jane", "jane@neurovision.ai");

        user.Id.Should().Be(id);
        user.UserName.Should().Be("doctor.jane");
        user.Email.Should().Be("jane@neurovision.ai");
        user.EmailConfirmed.Should().BeFalse();
        user.TwoFactorEnabled.Should().BeTrue();
        user.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        user.UpdatedAtUtc.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyId_GeneratesNewId()
    {
        var user = User.Create(Guid.Empty, "patient.john", "john@neurovision.ai");

        user.Id.Should().NotBe(Guid.Empty);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidUserName_Throws(string? userName)
    {
        var act = () => User.Create(Guid.NewGuid(), userName!, "user@neurovision.ai");

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidEmail_Throws(string? email)
    {
        var act = () => User.Create(Guid.NewGuid(), "username", email!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Restore_PreservesPersistedState()
    {
        var id = Guid.NewGuid();
        var created = DateTime.UtcNow.AddDays(-2);
        var updated = DateTime.UtcNow.AddHours(-1);

        var user = User.Restore(id, "superadmin", "admin@neurovision.ai", true, true, created, updated);

        user.Id.Should().Be(id);
        user.EmailConfirmed.Should().BeTrue();
        user.TwoFactorEnabled.Should().BeTrue();
        user.CreatedAtUtc.Should().Be(created);
        user.UpdatedAtUtc.Should().Be(updated);
    }
}
