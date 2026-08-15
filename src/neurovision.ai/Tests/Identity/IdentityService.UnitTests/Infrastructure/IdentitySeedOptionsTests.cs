using IdentityService.Infrastructure.Configuration;

namespace IdentityService.UnitTests.Infrastructure;

public class IdentitySeedOptionsTests
{
    [Fact]
    public void IsConfigured_WhenAllFieldsPresent_IsTrue()
    {
        var options = new SeedUserOptions
        {
            Email = "admin@neurovision.ai",
            UserName = "superadmin",
            Password = "Secret1"
        };

        options.IsConfigured.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "user", "pass")]
    [InlineData("a@b.c", "", "pass")]
    [InlineData("a@b.c", "user", "")]
    [InlineData("   ", "user", "pass")]
    public void IsConfigured_WhenAnyFieldMissing_IsFalse(string email, string userName, string password)
    {
        var options = new SeedUserOptions
        {
            Email = email,
            UserName = userName,
            Password = password
        };

        options.IsConfigured.Should().BeFalse();
    }
}
