namespace PdfService.UnitTests.Domain;

public class PdfTemplateFieldTests
{
    [Fact]
    public void Create_WithValidData_SetsIdentity()
    {
        var templateId = Guid.NewGuid();

        var field = PdfTemplateField.Create(
            "Signature",
            "Signature",
            page: 1,
            x: 10,
            y: 20,
            width: 100,
            height: 50,
            pdfTemplateId: templateId);

        field.Id.Should().NotBe(Guid.Empty);
        field.Name.Should().Be("Signature");
        field.Type.Should().Be("Signature");
        field.Page.Should().Be(1);
        field.PdfTemplateId.Should().Be(templateId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_Throws(string? name)
    {
        var act = () => PdfTemplateField.Create(name!, "Signature");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Restore_PreservesPersistedIdentity()
    {
        var id = Guid.NewGuid();
        var templateId = Guid.NewGuid();

        var field = PdfTemplateField.Restore(id, templateId, "Name", "Text", 2, 1, 2, 3, 4);

        field.Id.Should().Be(id);
        field.PdfTemplateId.Should().Be(templateId);
        field.Name.Should().Be("Name");
        field.Width.Should().Be(3);
    }
}
