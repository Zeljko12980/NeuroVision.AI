using PdfService.Grpc;

namespace MailService.Infrastructure.Services;

public class PdfDocumentGenerator : IDocumentGenerator
{
    private readonly PdfGenerator.PdfGeneratorClient _client;
    private readonly ILogger<PdfDocumentGenerator> _logger;

    public PdfDocumentGenerator(
        PdfGenerator.PdfGeneratorClient client,
        ILogger<PdfDocumentGenerator> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Result<byte[]>> GenerateAsync(
        string templateCode,
        IReadOnlyDictionary<string, string> placeholders,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GeneratePdfRequest { TemplateCode = templateCode };
            foreach (var (key, value) in placeholders)
                request.Placeholders.Add(key, value);

            var response = await _client.GeneratePdfAsync(request, cancellationToken: cancellationToken);
            if (!response.Success)
            {
                _logger.LogWarning(
                    "PDF generation failed. TemplateCode={TemplateCode}, Message={Message}",
                    templateCode,
                    response.Message);
                return Result<byte[]>.Fail(
                    $"PDF generation failed for template '{templateCode}': {response.Message}");
            }

            return Result<byte[]>.Ok(response.Pdf.ToByteArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PDF generation failed. TemplateCode={TemplateCode}", templateCode);
            return Result<byte[]>.Fail($"PDF generation failed for template '{templateCode}'.");
        }
    }
}
