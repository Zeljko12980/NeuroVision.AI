using BuildingBlocks.Pagination;
using BuildingBlocks.Persistence;
using Microsoft.Extensions.Logging.Abstractions;
using PdfService.Application.Common.Interfaces;
using PdfService.Application.Queries.Certificates;
using System.Net;

namespace PdfService.UnitTests.Application.Handlers;

public class GetCertificateByIdQueryHandlerTests
{
    private readonly IRepository<Certificate, Guid> _repository = Substitute.For<IRepository<Certificate, Guid>>();
    private readonly GetCertificateByIdQueryHandler _handler;

    public GetCertificateByIdQueryHandlerTests()
    {
        _handler = new GetCertificateByIdQueryHandler(
            _repository,
            NullLogger<GetCertificateByIdQueryHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenMissing_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Certificate?)null);

        var result = await _handler.Handle(new GetCertificateByIdQuery(id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Handle_WhenFound_MapsResponse()
    {
        var certificate = CertificateFactory.Create();
        _repository.GetByIdAsync(certificate.Id, Arg.Any<CancellationToken>()).Returns(certificate);

        var result = await _handler.Handle(new GetCertificateByIdQuery(certificate.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Thumbprint.Should().Be(certificate.Thumbprint);
        result.Value.Name.Should().Be("Doctor cert");
    }
}

public class GetAllCertificatesQueryHandlerTests
{
    private readonly ICertificateReadStore _readStore = Substitute.For<ICertificateReadStore>();
    private readonly GetAllCertificatesQueryHandler _handler;

    public GetAllCertificatesQueryHandlerTests()
    {
        _handler = new GetAllCertificatesQueryHandler(_readStore);
    }

    [Fact]
    public async Task Handle_ReturnsPagedCertificates()
    {
        var certificate = CertificateFactory.Create();
        _readStore.GetPagedAsync(0, 10, Arg.Any<CancellationToken>())
            .Returns((new List<Certificate> { certificate }, 1));

        var result = await _handler.Handle(
            new GetAllCertificatesQuery(new PaginationRequest(0, 10)),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Count.Should().Be(1);
        result.Value.Data.Should().ContainSingle(item => item.Thumbprint == certificate.Thumbprint);
    }

    [Fact]
    public async Task Handle_WhenPageIndexNegative_UsesZero()
    {
        _readStore.GetPagedAsync(0, 10, Arg.Any<CancellationToken>())
            .Returns((new List<Certificate>(), 0));

        var result = await _handler.Handle(
            new GetAllCertificatesQuery(new PaginationRequest(-2, 10)),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.PageIndex.Should().Be(0);
        await _readStore.Received(1).GetPagedAsync(0, 10, Arg.Any<CancellationToken>());
    }
}

internal static class CertificateFactory
{
    public static Certificate Create(
        string filePath = "certificates/doctor.pfx",
        DateTime? validTo = null) =>
        Certificate.Create(
            "Doctor cert",
            "CN=Doctor",
            "CN=CA",
            "ABC123",
            "1",
            DateTime.UtcNow.AddDays(-1),
            validTo ?? DateTime.UtcNow.AddYears(1),
            "doctor.pfx",
            filePath,
            "protected-secret");
}
