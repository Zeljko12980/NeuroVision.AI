using PdfService.Application.Common.Mappings;

namespace PdfService.UnitTests.Application.Mappings;

public class PdfMappingsTests
{
    [Fact]
    public void TemplateToResponse_MapsIdentityAndFields()
    {
        var template = PdfTemplate.Create(
            "SIGNED",
            "Signed report",
            "<div>{{Signature}}</div>",
            requiresSignature: true);

        var response = template.ToResponse();

        response.Id.Should().Be(template.Id);
        response.Code.Should().Be("SIGNED");
        response.Name.Should().Be("Signed report");
        response.RequiresSignature.Should().BeTrue();
        response.Fields.Should().ContainSingle(field => field.Type == PdfTemplate.SignatureFieldType);
    }

    [Fact]
    public void CertificateToResponse_MapsIdentityFields()
    {
        var certificate = Certificate.Create(
            "Doctor cert",
            "CN=Doctor",
            "CN=CA",
            "ABC123",
            "1",
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddYears(1),
            "doctor.pfx",
            "certificates/doctor.pfx",
            "protected-secret",
            isDefault: true);

        var response = certificate.ToResponse();

        response.Id.Should().Be(certificate.Id);
        response.Name.Should().Be("Doctor cert");
        response.Thumbprint.Should().Be("ABC123");
        response.FileName.Should().Be("doctor.pfx");
        response.IsDefault.Should().BeTrue();
        response.UserId.Should().BeNull();
        response.HasSignatureImage.Should().BeFalse();
    }

    [Fact]
    public void CertificateToResponse_MapsUserAndSignature()
    {
        var userId = Guid.NewGuid();
        var certificate = Certificate.Create(
            "Doctor cert",
            "CN=Doctor",
            "CN=CA",
            "ABC123",
            "1",
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddYears(1),
            "doctor.pfx",
            "certificates/doctor.pfx",
            "protected-secret",
            userId: userId,
            signatureImagePath: "signatures/sign.png");

        var response = certificate.ToResponse();

        response.UserId.Should().Be(userId);
        response.SignatureImagePath.Should().Be("signatures/sign.png");
        response.HasSignatureImage.Should().BeTrue();
    }
}
