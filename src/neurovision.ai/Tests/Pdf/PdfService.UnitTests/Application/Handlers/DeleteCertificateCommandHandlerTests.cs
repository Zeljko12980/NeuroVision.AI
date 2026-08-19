using BuildingBlocks.Persistence;
using Microsoft.Extensions.Logging.Abstractions;
using PdfService.Application.Commands.Certificates;
using PdfService.Application.Common.Interfaces;
using System.Net;

namespace PdfService.UnitTests.Application.Handlers;

public class DeleteCertificateCommandHandlerTests
{
    private readonly IRepository<Certificate, Guid> _repository = Substitute.For<IRepository<Certificate, Guid>>();
    private readonly ICertificateStorage _storage = Substitute.For<ICertificateStorage>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeleteCertificateCommandHandler _handler;

    public DeleteCertificateCommandHandlerTests()
    {
        _handler = new DeleteCertificateCommandHandler(
            _repository,
            _storage,
            _unitOfWork,
            NullLogger<DeleteCertificateCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenMissing_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Certificate?)null);

        var result = await _handler.Handle(new DeleteCertificateCommand(id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        _repository.DidNotReceive().Delete(Arg.Any<Certificate>());
    }

    [Fact]
    public async Task Handle_WhenFilePathPresent_DeletesFileAndRecord()
    {
        var certificate = CertificateFactory.Create("certificates/doctor.pfx");
        _repository.GetByIdAsync(certificate.Id, Arg.Any<CancellationToken>()).Returns(certificate);

        var result = await _handler.Handle(new DeleteCertificateCommand(certificate.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _storage.Received(1).DeleteAsync("certificates/doctor.pfx", Arg.Any<CancellationToken>());
        _repository.Received(1).Delete(certificate);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSignatureImagePresent_DeletesSignatureFile()
    {
        var certificate = CertificateFactory.Create(
            "certificates/doctor.pfx",
            signatureImagePath: "signatures/sign.png");
        _repository.GetByIdAsync(certificate.Id, Arg.Any<CancellationToken>()).Returns(certificate);

        var result = await _handler.Handle(new DeleteCertificateCommand(certificate.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _storage.Received(1).DeleteAsync("certificates/doctor.pfx", Arg.Any<CancellationToken>());
        await _storage.Received(1).DeleteAsync("signatures/sign.png", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenFilePathEmpty_SkipsStorageDelete()
    {
        var certificate = CertificateFactory.Create(filePath: " ");
        _repository.GetByIdAsync(certificate.Id, Arg.Any<CancellationToken>()).Returns(certificate);

        var result = await _handler.Handle(new DeleteCertificateCommand(certificate.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _storage.DidNotReceive().DeleteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        _repository.Received(1).Delete(certificate);
    }
}
