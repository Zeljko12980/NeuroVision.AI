using PdfService.Grpc;

namespace MailService.API.Services;

public class PdfServiceClient : IPdfServiceClient
{
    private readonly PdfGenerator.PdfGeneratorClient _client;

    public PdfServiceClient(PdfGenerator.PdfGeneratorClient client)
    {
        _client = client;
    }

    public async Task<GeneratePdfResponse> GeneratePdfAsync(
        string templateCode,
        IDictionary<string, string> placeholders,
        Guid? certificateId = null,
        CancellationToken cancellationToken = default)
    {
        var request = new GeneratePdfRequest
        {
            TemplateCode = templateCode,
            CertificateId = certificateId?.ToString() ?? string.Empty
        };

        foreach (var (key, value) in placeholders)
        {
            request.Placeholders.Add(key, value);
        }

        var response = await _client.GeneratePdfAsync(
            request,
            cancellationToken: cancellationToken);

        if (!response.Success)
        {
            throw new InvalidOperationException(
                $"PDF generation failed for template '{templateCode}': {response.Message}");
        }

        return response;
    }
}