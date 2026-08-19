using Microsoft.Extensions.Logging.Abstractions;
using PdfService.Infrastructure.Services;

namespace PdfService.UnitTests.Infrastructure;

public class HtmlPdfGeneratorTests
{
    [Fact]
    public async Task GenerateFromHtmlAsync_WithSimpleHtml_ReturnsPdfBytes()
    {
        var generator = new HtmlPdfGenerator(NullLogger<HtmlPdfGenerator>.Instance);

        var result = await generator.GenerateFromHtmlAsync("<html><body><p>NeuroVision</p></body></html>");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value.Take(4).Should().Equal(0x25, 0x50, 0x44, 0x46); // %PDF
    }

    [Fact]
    public async Task GenerateFromHtmlAsync_WhenConversionFails_ReturnsInternalServerError()
    {
        var generator = new HtmlPdfGenerator(NullLogger<HtmlPdfGenerator>.Instance);

        var result = await generator.GenerateFromHtmlAsync(null!);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("PDF generation failed.");
    }
}
