using BuildingBlocks.Persistence;
using Microsoft.Extensions.Logging.Abstractions;
using PdfService.Application.Common.Interfaces;
using PdfService.Application.Common.Models;
using PdfService.Infrastructure.Services;
using System.Net;

namespace PdfService.UnitTests.Infrastructure;

public class PdfSigningServiceTests
{
    private readonly IRepository<Certificate, Guid> _repository = Substitute.For<IRepository<Certificate, Guid>>();
    private readonly ICertificateStorage _storage = Substitute.For<ICertificateStorage>();
    private readonly ICertificatePasswordProtector _protector = Substitute.For<ICertificatePasswordProtector>();
    private readonly PdfSigningService _service;

    public PdfSigningServiceTests()
    {
        _service = new PdfSigningService(
            _repository,
            _storage,
            _protector,
            NullLogger<PdfSigningService>.Instance);
    }

    [Fact]
    public async Task SignPdfAsync_WhenPdfEmpty_ReturnsBadRequest()
    {
        var result = await _service.SignPdfAsync(
            [],
            Guid.NewGuid(),
            new SignaturePosition(),
            "reason",
            "location");

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await _repository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SignPdfAsync_WhenCertificateMissing_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Certificate?)null);

        var result = await _service.SignPdfAsync(
            [1, 2, 3],
            id,
            new SignaturePosition(),
            null,
            null);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SignPdfAsync_WhenCertificateExpired_ReturnsBadRequest()
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
        _repository.GetByIdAsync(certificate.Id, Arg.Any<CancellationToken>()).Returns(certificate);

        var result = await _service.SignPdfAsync(
            [1, 2, 3],
            certificate.Id,
            new SignaturePosition(),
            null,
            null);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cannot sign with an expired certificate.");
        await _storage.DidNotReceive().ReadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SignPdfAsync_WhenCertificateFileMissing_ReturnsInternalServerError()
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
        _repository.GetByIdAsync(certificate.Id, Arg.Any<CancellationToken>()).Returns(certificate);
        _storage.ReadAsync(certificate.FilePath, Arg.Any<CancellationToken>())
            .Returns<byte[]>(_ => throw new FileNotFoundException());

        var result = await _service.SignPdfAsync(
            [1, 2, 3],
            certificate.Id,
            new SignaturePosition(),
            null,
            null);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Failed to read the certificate file.");
    }

    [Fact]
    public async Task ResolvePosition_UsesLastPageWhenFieldHasNoPage()
    {
        var generator = new HtmlPdfGenerator(NullLogger<HtmlPdfGenerator>.Instance);
        var pdf = await generator.GenerateFromHtmlAsync("<html><body><p>NeuroVision</p></body></html>");
        var template = PdfTemplate.Create("MAIL", "Mail", "<p>hi</p>");

        var position = _service.ResolvePosition(pdf.Value, template);

        position.Page.Should().BeGreaterThan(0);
        position.Width.Should().Be(200f);
        position.Height.Should().Be(60f);
    }
}
