namespace PdfService.UnitTests.Domain;

public class CertificateTests
{
    [Fact]
    public void Create_WithValidData_SetsIdentity()
    {
        var validFrom = DateTime.UtcNow.AddDays(-1);
        var validTo = DateTime.UtcNow.AddYears(1);

        var certificate = Certificate.Create(
            "Doctor cert",
            "CN=Doctor",
            "CN=CA",
            "ABC123",
            "1",
            validFrom,
            validTo,
            "doctor.pfx",
            "certificates/doctor.pfx",
            "protected-secret");

        certificate.Id.Should().NotBe(Guid.Empty);
        certificate.Name.Should().Be("Doctor cert");
        certificate.Thumbprint.Should().Be("ABC123");
        certificate.UserId.Should().BeNull();
        certificate.SignatureImagePath.Should().BeNull();
    }

    [Fact]
    public void Create_WithUserAndSignature_PersistsAssignment()
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

        certificate.UserId.Should().Be(userId);
        certificate.SignatureImagePath.Should().Be("signatures/sign.png");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_Throws(string? name)
    {
        var act = () => Certificate.Create(
            name!,
            "CN=Doctor",
            "CN=CA",
            "ABC123",
            "1",
            DateTime.UtcNow,
            DateTime.UtcNow.AddYears(1),
            "doctor.pfx",
            "certificates/doctor.pfx",
            "protected-secret");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WhenValidToIsNotLater_Throws()
    {
        var instant = DateTime.UtcNow;

        var act = () => Certificate.Create(
            "Doctor cert",
            "CN=Doctor",
            "CN=CA",
            "ABC123",
            "1",
            instant,
            instant,
            "doctor.pfx",
            "certificates/doctor.pfx",
            "protected-secret");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void IsExpired_WhenValidToIsInThePast_ReturnsTrue()
    {
        var certificate = Certificate.Create(
            "Doctor cert",
            "CN=Doctor",
            "CN=CA",
            "ABC123",
            "1",
            DateTime.UtcNow.AddYears(-2),
            DateTime.UtcNow.AddDays(-1),
            "doctor.pfx",
            "certificates/doctor.pfx",
            "protected-secret");

        certificate.IsExpired().Should().BeTrue();
    }

    [Fact]
    public void Restore_PreservesPersistedIdentity()
    {
        var id = Guid.NewGuid();
        var validFrom = DateTime.UtcNow.AddDays(-1);
        var validTo = DateTime.UtcNow.AddYears(1);

        var certificate = Certificate.Restore(
            id,
            "Doctor cert",
            "CN=Doctor",
            "CN=CA",
            "ABC123",
            "1",
            validFrom,
            validTo,
            "doctor.pfx",
            "certificates/doctor.pfx",
            "protected-secret",
            isDefault: true);

        certificate.Id.Should().Be(id);
        certificate.IsDefault.Should().BeTrue();
        certificate.Thumbprint.Should().Be("ABC123");
    }

    [Fact]
    public void UpdateMetadata_ReplacesCertificateIdentity()
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
            "protected-secret");

        certificate.UpdateMetadata(
            "CN=Updated",
            "CN=NewCA",
            "DEF456",
            "2",
            DateTime.UtcNow.AddDays(-2),
            DateTime.UtcNow.AddYears(2));

        certificate.Subject.Should().Be("CN=Updated");
        certificate.Thumbprint.Should().Be("DEF456");
        certificate.SerialNumber.Should().Be("2");
    }

    [Fact]
    public void UpdateFilePath_UpdatesPathAndOptionalFileName()
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
            "certificates/old.pfx",
            "protected-secret");

        certificate.UpdateFilePath("certificates/new.pfx", "new.pfx");

        certificate.FilePath.Should().Be("certificates/new.pfx");
        certificate.FileName.Should().Be("new.pfx");
    }

    [Fact]
    public void UpdateProtectedPassword_WhenEmpty_Throws()
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
            "protected-secret");

        var act = () => certificate.UpdateProtectedPassword(" ");

        act.Should().Throw<ArgumentException>();
    }
}
