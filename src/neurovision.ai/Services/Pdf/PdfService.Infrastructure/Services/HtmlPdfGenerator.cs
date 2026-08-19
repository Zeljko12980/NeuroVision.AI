using iText.Html2pdf;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using PdfService.Application.Common.Interfaces;
using BuildingBlocks.Results;
using System.Net;

namespace PdfService.Infrastructure.Services;

public sealed class HtmlPdfGenerator : IPdfGenerator
{
    private readonly ILogger<HtmlPdfGenerator> _logger;
    private readonly string? _webRootPath;

    public HtmlPdfGenerator(
        ILogger<HtmlPdfGenerator> logger,
        IWebHostEnvironment? environment = null)
    {
        _logger = logger;
        _webRootPath = environment?.WebRootPath;
    }

    public Task<Result<byte[]>> GenerateFromHtmlAsync(
        string html,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            using var writer = new PdfWriter(memoryStream);
            using var pdfDocument = new PdfDocument(writer);

            var properties = new ConverterProperties();
            if (!string.IsNullOrWhiteSpace(_webRootPath))
                properties.SetBaseUri(_webRootPath + Path.DirectorySeparatorChar);

            HtmlConverter.ConvertToPdf(html, pdfDocument, properties);
            pdfDocument.Close();

            return Task.FromResult(Result<byte[]>.Ok(memoryStream.ToArray()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HTML to PDF conversion failed.");
            return Task.FromResult(Result<byte[]>.Fail(
                "PDF generation failed.",
                HttpStatusCode.InternalServerError));
        }
    }
}
