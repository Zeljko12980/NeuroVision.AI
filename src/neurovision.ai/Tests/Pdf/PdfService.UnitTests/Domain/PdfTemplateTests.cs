namespace PdfService.UnitTests.Domain;

public class PdfTemplateTests
{
    [Fact]
    public void Create_WithValidData_SetsIdentityAndTimestamps()
    {
        var template = PdfTemplate.Create("REPORT", "Tumor report", "<p>{{Patient}}</p>");

        template.Id.Should().NotBe(Guid.Empty);
        template.Code.Should().Be("REPORT");
        template.Name.Should().Be("Tumor report");
        template.HtmlContent.Should().Be("<p>{{Patient}}</p>");
        template.Version.Should().Be(1);
        template.IsActive.Should().BeTrue();
        template.RequiresSignature.Should().BeFalse();
        template.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        template.Fields.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidCode_Throws(string? code)
    {
        var act = () => PdfTemplate.Create(code!, "Name", "<p>hi</p>");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WhenSignatureRequired_AddsDefaultSignatureField()
    {
        var template = PdfTemplate.Create(
            "SIGNED",
            "Signed report",
            "<div>{{Signature}}</div>",
            requiresSignature: true);

        var field = template.GetSignatureField();
        field.Should().NotBeNull();
        field!.Type.Should().Be(PdfTemplate.SignatureFieldType);
        field.PdfTemplateId.Should().Be(template.Id);
    }

    [Fact]
    public void RenderHtml_ReplacesModelAndMustachePlaceholders()
    {
        var template = PdfTemplate.Create(
            "MAIL",
            "Mail",
            "<p>{{FullName}}</p><a href=\"@Model.ConfirmationUrl\">confirm</a>");

        var html = template.RenderHtml(new Dictionary<string, string>
        {
            ["FullName"] = "Jane",
            ["ConfirmationUrl"] = "https://app.neurovision.ai/confirm"
        });

        html.Should().Contain(">Jane<");
        html.Should().Contain("https://app.neurovision.ai/confirm");
        html.Should().NotContain("{{FullName}}");
        html.Should().NotContain("@Model.ConfirmationUrl");
    }

    [Fact]
    public void RenderHtml_WhenKeysAreModelPrefixed_ReplacesPlaceholders()
    {
        var template = PdfTemplate.Create(
            "SECURITY_CODE",
            "Security Code",
            "<p>Hello @Model.FullName,</p><div>@Model.Code</div>");

        var html = template.RenderHtml(new Dictionary<string, string>
        {
            ["@Model.FullName"] = "Jane",
            ["@Model.Code"] = "123456"
        });

        html.Should().Contain("Jane");
        html.Should().Contain("123456");
        html.Should().NotContain("@Model.FullName");
        html.Should().NotContain("@Model.Code");
    }

    [Fact]
    public void RenderHtml_DoesNotReplaceRawKeyInsideMarkup()
    {
        var template = PdfTemplate.Create("MAIL", "Mail", "<p class=\"Name\">{{Name}}</p>");

        var html = template.RenderHtml(new Dictionary<string, string>
        {
            ["Name"] = "Jane"
        });

        html.Should().Contain("class=\"Name\"");
        html.Should().Contain(">Jane<");
    }

    [Fact]
    public void RenderHtml_WhenSignatureImageProvided_InsertsImage()
    {
        var template = PdfTemplate.Create(
            "SIGNED",
            "Signed",
            "<div>{{Signature}}</div>",
            requiresSignature: true);

        var html = template.RenderHtml(new Dictionary<string, string>(), [1, 2, 3]);

        html.Should().Contain("data:image/png;base64,");
        html.Should().NotContain(PdfTemplate.SignaturePlaceholder);
    }

    [Fact]
    public void Update_ChangesMutableFields()
    {
        var template = PdfTemplate.Create("CODE", "Old", "<p>old</p>");

        template.Update("New", "<p>new</p>", 2, false);

        template.Name.Should().Be("New");
        template.HtmlContent.Should().Be("<p>new</p>");
        template.Version.Should().Be(2);
        template.IsActive.Should().BeFalse();
        template.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_WithInvalidName_Throws()
    {
        var template = PdfTemplate.Create("CODE", "Name", "<p>old</p>");

        var act = () => template.Update("  ", "<p>new</p>", 2, true);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Restore_PreservesPersistedState()
    {
        var id = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow.AddDays(-2);
        var updatedAt = DateTimeOffset.UtcNow.AddDays(-1);

        var template = PdfTemplate.Restore(
            id,
            "MAIL",
            "Mail",
            "<p>hi</p>",
            3,
            false,
            createdAt,
            updatedAt,
            requiresSignature: false,
            signaturePage: 1);

        template.Id.Should().Be(id);
        template.Version.Should().Be(3);
        template.IsActive.Should().BeFalse();
        template.CreatedAt.Should().Be(createdAt);
        template.UpdatedAt.Should().Be(updatedAt);
        template.Fields.Should().BeEmpty();
    }

    [Fact]
    public void EnsureSignatureField_DoesNotDuplicateExistingField()
    {
        var template = PdfTemplate.Create(
            "SIGNED",
            "Signed",
            "<div>{{Signature}}</div>",
            requiresSignature: true);

        template.EnsureSignatureField();
        template.EnsureSignatureField();

        template.Fields.Count(field => field.Type == PdfTemplate.SignatureFieldType).Should().Be(1);
    }

    [Fact]
    public void RenderHtml_WhenSignaturePlaceholderHasNoImage_RemovesPlaceholder()
    {
        var template = PdfTemplate.Create(
            "SIGNED",
            "Signed",
            "<div>{{Signature}}</div>",
            requiresSignature: true);

        var html = template.RenderHtml(new Dictionary<string, string>());

        html.Should().NotContain(PdfTemplate.SignaturePlaceholder);
        html.Should().NotContain("data:image/png;base64,");
    }

    [Fact]
    public void Create_WhenVersionNotPositive_Throws()
    {
        var act = () => PdfTemplate.Create("CODE", "Name", "<p>hi</p>", version: 0);

        act.Should().Throw<ArgumentException>();
    }
}
