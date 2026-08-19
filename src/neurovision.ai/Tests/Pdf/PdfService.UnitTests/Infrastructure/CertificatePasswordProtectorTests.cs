using Microsoft.AspNetCore.DataProtection;
using PdfService.Infrastructure.Services;

namespace PdfService.UnitTests.Infrastructure;

public class CertificatePasswordProtectorTests
{
    [Fact]
    public void Protect_ThenUnprotect_ReturnsOriginalPassword()
    {
        var protector = new CertificatePasswordProtector(new EphemeralDataProtectionProvider());

        var protectedPassword = protector.Protect("secret");
        var roundTripped = protector.Unprotect(protectedPassword);

        protectedPassword.Should().NotBe("secret");
        roundTripped.Should().Be("secret");
    }
}
