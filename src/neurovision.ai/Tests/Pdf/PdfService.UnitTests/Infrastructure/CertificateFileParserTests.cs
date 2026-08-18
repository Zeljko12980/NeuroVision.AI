using PdfService.Infrastructure.Services;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace PdfService.UnitTests.Infrastructure;

public class CertificateFileParserTests
{
    private readonly CertificateFileParser _parser = new();

    [Fact]
    public void Parse_WhenFileIsNotACertificate_ReturnsBadRequest()
    {
        var result = _parser.Parse([1, 2, 3, 4], "secret");

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Parse_WhenPasswordIsWrong_ReturnsBadRequest()
    {
        var pfx = CreatePfx("correct-password");

        var result = _parser.Parse(pfx, "wrong-password");

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public void Parse_WhenValidPfx_ReturnsMetadata()
    {
        var pfx = CreatePfx("secret");

        var result = _parser.Parse(pfx, "secret");

        result.IsSuccess.Should().BeTrue();
        result.Value.Subject.Should().Contain("CN=NeuroVision Test");
        result.Value.Thumbprint.Should().NotBeNullOrWhiteSpace();
        result.Value.ValidTo.Should().BeAfter(DateTime.UtcNow);
    }

    private static byte[] CreatePfx(string password)
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(
            "CN=NeuroVision Test",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);
        using var certificate = request.CreateSelfSigned(
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddYears(1));

        return certificate.Export(X509ContentType.Pfx, password);
    }
}
