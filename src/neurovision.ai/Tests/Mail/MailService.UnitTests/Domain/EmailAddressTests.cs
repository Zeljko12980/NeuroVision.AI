using MailService.Domain.ValueObjects;

namespace MailService.UnitTests.Domain;

public class EmailAddressTests
{
    [Theory]
    [InlineData("jane@neurovision.ai")]
    [InlineData("  jane@neurovision.ai  ")]
    public void Create_WithValidEmail_Normalizes(string value)
    {
        var email = EmailAddress.Create(value);

        email.Value.Should().Be("jane@neurovision.ai");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    [InlineData("missing-domain@")]
    public void Create_WithInvalidEmail_Throws(string? value)
    {
        var act = () => EmailAddress.Create(value!);

        act.Should().Throw<ArgumentException>();
    }
}
