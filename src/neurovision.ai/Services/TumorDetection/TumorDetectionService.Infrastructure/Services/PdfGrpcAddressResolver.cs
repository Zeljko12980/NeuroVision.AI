using Microsoft.Extensions.Configuration;
using TumorDetectionService.Application.Common.Options;

namespace TumorDetectionService.Infrastructure.Services;

internal static class PdfGrpcAddressResolver
{
    public static Uri Resolve(IConfiguration configuration, PdfServiceOptions options)
    {
        foreach (var key in new[]
                 {
                     "PdfService:GrpcUrl",
                     "services:pdfservice-api:grpc:0",
                     "Services:pdfservice-api:grpc:0",
                 })
        {
            var endpoint = configuration[key];
            if (!string.IsNullOrWhiteSpace(endpoint))
                return new Uri(endpoint);
        }

        return new Uri(options.GrpcUrl);
    }
}
