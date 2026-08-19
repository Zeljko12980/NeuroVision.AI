using PdfService.Application.Commands.Certificates;
using PdfService.Application.Commands.Templates;
using PdfService.Application.Common.Requests;

namespace PdfService.UnitTests.Application.Validators;

public class CreatePdfTemplateCommandValidatorTests
{
    private readonly CreatePdfTemplateCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenValid_Succeeds()
    {
        var result = _validator.Validate(new CreatePdfTemplateCommand(new PdfTemplateRequest
        {
            Code = "MAIL",
            Name = "Mail",
            HtmlContent = "<p>hi</p>",
            Version = 1
        }));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenCodeMissing_Fails()
    {
        var result = _validator.Validate(new CreatePdfTemplateCommand(new PdfTemplateRequest
        {
            Code = "",
            Name = "Mail",
            HtmlContent = "<p>hi</p>",
            Version = 1
        }));

        result.IsValid.Should().BeFalse();
    }
}

public class CreateCertificateCommandValidatorTests
{
    private readonly CreateCertificateCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenValidPfx_Succeeds()
    {
        var result = _validator.Validate(new CreateCertificateCommand(
            Guid.NewGuid(),
            "Doctor cert",
            "secret",
            [1, 2, 3],
            "doctor.pfx",
            [9, 9],
            "sign.png"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenExtensionNotAllowed_Fails()
    {
        var result = _validator.Validate(new CreateCertificateCommand(
            Guid.NewGuid(),
            "Doctor cert",
            "secret",
            [1, 2, 3],
            "doctor.txt",
            [9, 9],
            "sign.png"));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhenFileEmpty_Fails()
    {
        var result = _validator.Validate(new CreateCertificateCommand(
            Guid.NewGuid(),
            "Doctor cert",
            "secret",
            [],
            "doctor.pfx",
            [9, 9],
            "sign.png"));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhenNameMissing_Fails()
    {
        var result = _validator.Validate(new CreateCertificateCommand(
            Guid.NewGuid(),
            "",
            "secret",
            [1, 2, 3],
            "doctor.pfx",
            [9, 9],
            "sign.png"));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhenUserIdMissing_Fails()
    {
        var result = _validator.Validate(new CreateCertificateCommand(
            Guid.Empty,
            "Doctor cert",
            "secret",
            [1, 2, 3],
            "doctor.pfx",
            [9, 9],
            "sign.png"));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhenSignatureImageMissing_Fails()
    {
        var result = _validator.Validate(new CreateCertificateCommand(
            Guid.NewGuid(),
            "Doctor cert",
            "secret",
            [1, 2, 3],
            "doctor.pfx",
            [],
            "sign.png"));

        result.IsValid.Should().BeFalse();
    }
}

public class UpdatePdfTemplateCommandValidatorTests
{
    private readonly UpdatePdfTemplateCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenValid_Succeeds()
    {
        var result = _validator.Validate(new UpdatePdfTemplateCommand(
            Guid.NewGuid(),
            new UpdatePdfTemplateRequest
            {
                Name = "Mail",
                HtmlContent = "<p>hi</p>",
                Version = 1,
                IsActive = true
            }));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenIdEmpty_Fails()
    {
        var result = _validator.Validate(new UpdatePdfTemplateCommand(
            Guid.Empty,
            new UpdatePdfTemplateRequest
            {
                Name = "Mail",
                HtmlContent = "<p>hi</p>",
                Version = 1,
                IsActive = true
            }));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WhenVersionNotPositive_Fails()
    {
        var result = _validator.Validate(new UpdatePdfTemplateCommand(
            Guid.NewGuid(),
            new UpdatePdfTemplateRequest
            {
                Name = "Mail",
                HtmlContent = "<p>hi</p>",
                Version = 0,
                IsActive = true
            }));

        result.IsValid.Should().BeFalse();
    }
}

public class GeneratePdfCommandValidatorTests
{
    private readonly GeneratePdfCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenValid_Succeeds()
    {
        var result = _validator.Validate(new GeneratePdfCommand("MAIL", [], null));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenTemplateCodeMissing_Fails()
    {
        var result = _validator.Validate(new GeneratePdfCommand("", [], null));

        result.IsValid.Should().BeFalse();
    }
}
