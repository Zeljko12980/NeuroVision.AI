using BuildingBlocks.Persistence;
using Microsoft.Extensions.Logging.Abstractions;
using PdfService.Application.Commands.Certificates;
using PdfService.Application.Commands.Templates;
using PdfService.Application.Common.Interfaces;
using PdfService.Application.Common.Models;
using PdfService.Application.Common.Requests;
using System.Net;

namespace PdfService.UnitTests.Application.Handlers;

public class GeneratePdfCommandHandlerTests
{
    private readonly IPdfTemplateReadStore _readStore = Substitute.For<IPdfTemplateReadStore>();
    private readonly ICertificateReadStore _certificateReadStore = Substitute.For<ICertificateReadStore>();
    private readonly IPdfGenerator _pdfGenerator = Substitute.For<IPdfGenerator>();
    private readonly IPdfSigningService _signingService = Substitute.For<IPdfSigningService>();
    private readonly ICertificateStorage _storage = Substitute.For<ICertificateStorage>();
    private readonly GeneratePdfCommandHandler _handler;

    public GeneratePdfCommandHandlerTests()
    {
        _handler = new GeneratePdfCommandHandler(
            _readStore,
            _certificateReadStore,
            _pdfGenerator,
            _signingService,
            _storage,
            NullLogger<GeneratePdfCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenTemplateMissing_ReturnsNotFound()
    {
        _readStore.GetByCodeAsync("MISSING", Arg.Any<CancellationToken>())
            .Returns((PdfTemplate?)null);

        var result = await _handler.Handle(UnsignedCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await _pdfGenerator.DidNotReceive().GenerateFromHtmlAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUnsignedTemplate_GeneratesPdfWithoutSigning()
    {
        var template = PdfTemplate.Create("MAIL", "Mail", "<p>{{Name}}</p>");
        _readStore.GetByCodeAsync("MAIL", Arg.Any<CancellationToken>()).Returns(template);
        _pdfGenerator.GenerateFromHtmlAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<byte[]>.Ok([1, 2, 3]));

        var result = await _handler.Handle(
            new GeneratePdfCommand("MAIL", new Dictionary<string, string> { ["Name"] = "Jane" }, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.PdfBytes.Should().Equal(1, 2, 3);
        result.Value.IsSigned.Should().BeFalse();
        await _signingService.DidNotReceive().SignPdfAsync(
            Arg.Any<byte[]>(),
            Arg.Any<Guid>(),
            Arg.Any<SignaturePosition>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSignatureRequiredWithoutCertificate_ReturnsBadRequest()
    {
        var template = PdfTemplate.Create(
            "SIGNED",
            "Signed",
            "<div>{{Signature}}</div>",
            requiresSignature: true);
        _readStore.GetByCodeAsync("SIGNED", Arg.Any<CancellationToken>()).Returns(template);

        var result = await _handler.Handle(
            new GeneratePdfCommand("SIGNED", [], null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await _pdfGenerator.DidNotReceive().GenerateFromHtmlAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSignatureRequired_SignsGeneratedPdf()
    {
        var certificateId = Guid.NewGuid();
        var signingCertificate = Certificate.Create(
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
            id: certificateId);
        var template = PdfTemplate.Create(
            "SIGNED",
            "Signed",
            "<div>{{Signature}}</div>",
            requiresSignature: true);
        var position = new SignaturePosition { Page = 1, X = 10, Y = 20, Width = 100, Height = 50 };

        _readStore.GetByCodeAsync("SIGNED", Arg.Any<CancellationToken>()).Returns(template);
        _certificateReadStore.GetByIdAsync(certificateId, Arg.Any<CancellationToken>())
            .Returns(signingCertificate);
        _storage.TryReadSignatureImageAsync("signature.png", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<byte[]?>([9, 9]));
        _pdfGenerator.GenerateFromHtmlAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<byte[]>.Ok([1, 2, 3]));
        _signingService.ResolvePosition(Arg.Any<byte[]>(), template).Returns(position);
        _signingService.SignPdfAsync(
                Arg.Any<byte[]>(),
                certificateId,
                position,
                Arg.Any<string?>(),
                Arg.Any<string?>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<byte[]>.Ok([4, 5, 6]));

        var result = await _handler.Handle(
            new GeneratePdfCommand("SIGNED", [], certificateId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsSigned.Should().BeTrue();
        result.Value.PdfBytes.Should().Equal(4, 5, 6);
        result.Value.CertificateId.Should().Be(certificateId);
    }

    [Fact]
    public async Task Handle_WhenUserIdProvided_SignsWithUserCertificate()
    {
        var userId = Guid.NewGuid();
        var certificateId = Guid.NewGuid();
        var signingCertificate = Certificate.Create(
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
            signatureImagePath: "signatures/doctor.png",
            id: certificateId);
        var template = PdfTemplate.Create(
            "SIGNED",
            "Signed",
            "<div>{{Signature}}</div>",
            requiresSignature: true);
        var position = new SignaturePosition { Page = 1, X = 10, Y = 20, Width = 100, Height = 50 };

        _readStore.GetByCodeAsync("SIGNED", Arg.Any<CancellationToken>()).Returns(template);
        _certificateReadStore.GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(signingCertificate);
        _storage.TryReadSignatureImageAsync("signatures/doctor.png", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<byte[]?>([9, 9]));
        _pdfGenerator.GenerateFromHtmlAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<byte[]>.Ok([1, 2, 3]));
        _signingService.ResolvePosition(Arg.Any<byte[]>(), template).Returns(position);
        _signingService.SignPdfAsync(
                Arg.Any<byte[]>(),
                certificateId,
                position,
                Arg.Any<string?>(),
                Arg.Any<string?>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<byte[]>.Ok([4, 5, 6]));

        var result = await _handler.Handle(
            new GeneratePdfCommand("SIGNED", [], Guid.NewGuid(), userId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsSigned.Should().BeTrue();
        result.Value.CertificateId.Should().Be(certificateId);
        await _certificateReadStore.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenGeneratorFails_ReturnsFailure()
    {
        var template = PdfTemplate.Create("MAIL", "Mail", "<p>{{Name}}</p>");
        _readStore.GetByCodeAsync("MAIL", Arg.Any<CancellationToken>()).Returns(template);
        _pdfGenerator.GenerateFromHtmlAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<byte[]>.Fail("conversion failed", HttpStatusCode.InternalServerError));

        var result = await _handler.Handle(
            new GeneratePdfCommand("MAIL", new Dictionary<string, string> { ["Name"] = "Jane" }, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("conversion failed");
        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        await _signingService.DidNotReceive().SignPdfAsync(
            Arg.Any<byte[]>(),
            Arg.Any<Guid>(),
            Arg.Any<SignaturePosition>(),
            Arg.Any<string?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSigningFails_ReturnsFailure()
    {
        var certificateId = Guid.NewGuid();
        var signingCertificate = Certificate.Create(
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
            id: certificateId);
        var template = PdfTemplate.Create(
            "SIGNED",
            "Signed",
            "<div>{{Signature}}</div>",
            requiresSignature: true);
        var position = new SignaturePosition { Page = 1, X = 10, Y = 20, Width = 100, Height = 50 };

        _readStore.GetByCodeAsync("SIGNED", Arg.Any<CancellationToken>()).Returns(template);
        _certificateReadStore.GetByIdAsync(certificateId, Arg.Any<CancellationToken>())
            .Returns(signingCertificate);
        _pdfGenerator.GenerateFromHtmlAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<byte[]>.Ok([1, 2, 3]));
        _signingService.ResolvePosition(Arg.Any<byte[]>(), template).Returns(position);
        _signingService.SignPdfAsync(
                Arg.Any<byte[]>(),
                certificateId,
                position,
                Arg.Any<string?>(),
                Arg.Any<string?>(),
                Arg.Any<CancellationToken>())
            .Returns(Result<byte[]>.Fail("Certificate not found.", HttpStatusCode.NotFound));

        var result = await _handler.Handle(
            new GeneratePdfCommand("SIGNED", [], certificateId),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Certificate not found.");
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Handle_WhenGeneratorThrows_ReturnsInternalServerError()
    {
        var template = PdfTemplate.Create("MAIL", "Mail", "<p>hi</p>");
        _readStore.GetByCodeAsync("MAIL", Arg.Any<CancellationToken>()).Returns(template);
        _pdfGenerator.GenerateFromHtmlAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns<Result<byte[]>>(_ => throw new InvalidOperationException("boom"));

        var result = await _handler.Handle(new GeneratePdfCommand("MAIL", [], null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("PDF generation failed.");
        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    private static GeneratePdfCommand UnsignedCommand() =>
        new("MISSING", [], null);
}

public class CreatePdfTemplateCommandHandlerTests
{
    private readonly IPdfTemplateReadStore _readStore = Substitute.For<IPdfTemplateReadStore>();
    private readonly IRepository<PdfTemplate, Guid> _repository = Substitute.For<IRepository<PdfTemplate, Guid>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreatePdfTemplateCommandHandler _handler;

    public CreatePdfTemplateCommandHandlerTests()
    {
        _handler = new CreatePdfTemplateCommandHandler(_readStore, _repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenCodeExists_ReturnsConflict()
    {
        _readStore.GetIdByCodeAsync("MAIL", Arg.Any<CancellationToken>()).Returns(Guid.NewGuid());

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        await _repository.DidNotReceive().AddAsync(Arg.Any<PdfTemplate>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCodeIsNew_CreatesTemplate()
    {
        _readStore.GetIdByCodeAsync("MAIL", Arg.Any<CancellationToken>()).Returns((Guid?)null);

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Value.Code.Should().Be("MAIL");
        await _repository.Received(1).AddAsync(Arg.Any<PdfTemplate>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static CreatePdfTemplateCommand ValidCommand() =>
        new(new PdfTemplateRequest
        {
            Code = "MAIL",
            Name = "Mail",
            HtmlContent = "<p>{{Name}}</p>",
            Version = 1
        });
}

public class CreateCertificateCommandHandlerTests
{
    private readonly ICertificateFileParser _parser = Substitute.For<ICertificateFileParser>();
    private readonly ICertificateStorage _storage = Substitute.For<ICertificateStorage>();
    private readonly ICertificatePasswordProtector _protector = Substitute.For<ICertificatePasswordProtector>();
    private readonly ICertificateReadStore _readStore = Substitute.For<ICertificateReadStore>();
    private readonly IRepository<Certificate, Guid> _repository = Substitute.For<IRepository<Certificate, Guid>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateCertificateCommandHandler _handler;

    public CreateCertificateCommandHandlerTests()
    {
        _handler = new CreateCertificateCommandHandler(
            _parser,
            _storage,
            _protector,
            _readStore,
            _repository,
            _unitOfWork,
            NullLogger<CreateCertificateCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenParseFails_DoesNotSave()
    {
        _parser.Parse(Arg.Any<byte[]>(), Arg.Any<string?>())
            .Returns(Result<ParsedCertificate>.Fail("bad password", HttpStatusCode.BadRequest));

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        await _storage.DidNotReceive().SaveAsync(Arg.Any<byte[]>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCertificateExpired_DoesNotSave()
    {
        _parser.Parse(Arg.Any<byte[]>(), Arg.Any<string?>())
            .Returns(Result<ParsedCertificate>.Ok(new ParsedCertificate
            {
                Subject = "CN=Doctor",
                Issuer = "CN=CA",
                Thumbprint = "ABC",
                SerialNumber = "1",
                ValidFrom = DateTime.UtcNow.AddYears(-2),
                ValidTo = DateTime.UtcNow.AddDays(-1)
            }));

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("The certificate has already expired.");
        await _storage.DidNotReceive().SaveAsync(Arg.Any<byte[]>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenValid_PersistsCertificate()
    {
        _parser.Parse(Arg.Any<byte[]>(), Arg.Any<string?>())
            .Returns(Result<ParsedCertificate>.Ok(new ParsedCertificate
            {
                Subject = "CN=Doctor",
                Issuer = "CN=CA",
                Thumbprint = "ABC",
                SerialNumber = "1",
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidTo = DateTime.UtcNow.AddYears(1)
            }));
        _storage.SaveAsync(Arg.Any<byte[]>(), "doctor.pfx", Arg.Any<CancellationToken>())
            .Returns("certificates/abc.pfx");
        _storage.SaveSignatureImageAsync(Arg.Any<byte[]>(), "sign.png", Arg.Any<CancellationToken>())
            .Returns("signatures/sign.png");
        _protector.Protect(Arg.Any<string>()).Returns("protected");

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Thumbprint.Should().Be("ABC");
        result.Value.HasSignatureImage.Should().BeTrue();
        result.Value.UserId.Should().NotBeNull();
        await _repository.Received(1).AddAsync(Arg.Any<Certificate>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserAlreadyHasCertificate_ReturnsConflict()
    {
        _readStore.ExistsForUserAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Error.Should().Be("A signing certificate already exists for this user.");
        await _storage.DidNotReceive().SaveAsync(Arg.Any<byte[]>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSaveThrows_DoesNotPersist()
    {
        _parser.Parse(Arg.Any<byte[]>(), Arg.Any<string?>())
            .Returns(Result<ParsedCertificate>.Ok(ValidParsed()));
        _storage.SaveAsync(Arg.Any<byte[]>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns<string>(_ => throw new IOException("disk full"));

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Failed to save the certificate file.");
        await _repository.DidNotReceive().AddAsync(Arg.Any<Certificate>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDomainCreateFails_DeletesStoredFile()
    {
        _parser.Parse(Arg.Any<byte[]>(), Arg.Any<string?>())
            .Returns(Result<ParsedCertificate>.Ok(new ParsedCertificate
            {
                Subject = "CN=Doctor",
                Issuer = "CN=CA",
                Thumbprint = "",
                SerialNumber = "1",
                ValidFrom = DateTime.UtcNow.AddDays(-1),
                ValidTo = DateTime.UtcNow.AddYears(1)
            }));
        _storage.SaveAsync(Arg.Any<byte[]>(), "doctor.pfx", Arg.Any<CancellationToken>())
            .Returns("certificates/abc.pfx");
        _storage.SaveSignatureImageAsync(Arg.Any<byte[]>(), "sign.png", Arg.Any<CancellationToken>())
            .Returns("signatures/sign.png");
        _protector.Protect(Arg.Any<string>()).Returns("protected");

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await _storage.Received(1).DeleteAsync("certificates/abc.pfx", Arg.Any<CancellationToken>());
        await _storage.Received(1).DeleteAsync("signatures/sign.png", Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().AddAsync(Arg.Any<Certificate>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDatabaseSaveFails_DeletesStoredFile()
    {
        _parser.Parse(Arg.Any<byte[]>(), Arg.Any<string?>())
            .Returns(Result<ParsedCertificate>.Ok(ValidParsed()));
        _storage.SaveAsync(Arg.Any<byte[]>(), "doctor.pfx", Arg.Any<CancellationToken>())
            .Returns("certificates/abc.pfx");
        _storage.SaveSignatureImageAsync(Arg.Any<byte[]>(), "sign.png", Arg.Any<CancellationToken>())
            .Returns("signatures/sign.png");
        _protector.Protect(Arg.Any<string>()).Returns("protected");
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns<int>(_ => throw new InvalidOperationException("db down"));

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Failed to save the certificate record.");
        await _storage.Received(1).DeleteAsync("certificates/abc.pfx", Arg.Any<CancellationToken>());
        await _storage.Received(1).DeleteAsync("signatures/sign.png", Arg.Any<CancellationToken>());
    }

    private static ParsedCertificate ValidParsed() =>
        new()
        {
            Subject = "CN=Doctor",
            Issuer = "CN=CA",
            Thumbprint = "ABC",
            SerialNumber = "1",
            ValidFrom = DateTime.UtcNow.AddDays(-1),
            ValidTo = DateTime.UtcNow.AddYears(1)
        };

    private static CreateCertificateCommand ValidCommand() =>
        new(Guid.NewGuid(), "Doctor cert", "secret", [1, 2, 3], "doctor.pfx", [9, 9], "sign.png");
}
