using PdfService.Grpc;

namespace MailService.API.Services.Interfaces
{
    public interface IPdfServiceClient
    {
        Task<GeneratePdfResponse> GeneratePdfAsync(
            string templateCode,
            IDictionary<string, string> placeholders,
            Guid? certificateId = null,
            CancellationToken cancellationToken = default);
    }
}
