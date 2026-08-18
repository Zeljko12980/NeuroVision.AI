using Grpc.Core;
using MediatR;
using PdfService.Application.Commands.Templates;
using GrpcGeneratePdfRequest = PdfService.Grpc.GeneratePdfRequest;
using GrpcGeneratePdfResponse = PdfService.Grpc.GeneratePdfResponse;

namespace PdfService.API.Services;

public class PdfGeneratorService : PdfService.Grpc.PdfGenerator.PdfGeneratorBase
{
    private readonly ISender _sender;

    public PdfGeneratorService(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<GrpcGeneratePdfResponse> GeneratePdf(
        GrpcGeneratePdfRequest request,
        ServerCallContext context)
    {
        Guid? certificateId = null;
        if (!string.IsNullOrWhiteSpace(request.CertificateId))
        {
            if (!Guid.TryParse(request.CertificateId, out var parsedId))
            {
                return new GrpcGeneratePdfResponse
                {
                    Success = false,
                    Message = "Invalid certificate ID.",
                    Pdf = Google.Protobuf.ByteString.Empty
                };
            }

            certificateId = parsedId;
        }

        Guid? userId = null;
        if (!string.IsNullOrWhiteSpace(request.UserId))
        {
            if (!Guid.TryParse(request.UserId, out var parsedUserId))
            {
                return new GrpcGeneratePdfResponse
                {
                    Success = false,
                    Message = "Invalid user ID.",
                    Pdf = Google.Protobuf.ByteString.Empty
                };
            }

            userId = parsedUserId;
        }

        var result = await _sender.Send(
            new GeneratePdfCommand(
                request.TemplateCode,
                request.Placeholders.ToDictionary(x => x.Key, x => x.Value),
                certificateId,
                userId),
            context.CancellationToken);

        if (!result.IsSuccess)
        {
            return new GrpcGeneratePdfResponse
            {
                Success = false,
                Message = result.Error,
                Pdf = Google.Protobuf.ByteString.Empty
            };
        }

        return new GrpcGeneratePdfResponse
        {
            Success = true,
            Message = "PDF generated successfully.",
            Pdf = Google.Protobuf.ByteString.CopyFrom(result.Value.PdfBytes),
            IsSigned = result.Value.IsSigned,
            CertificateId = result.Value.CertificateId?.ToString() ?? string.Empty,
            GeneratedAt = result.Value.GeneratedAt.ToString("O"),
            SignatureReason = result.Value.SignatureReason ?? string.Empty,
            SignatureLocation = result.Value.SignatureLocation ?? string.Empty
        };
    }
}
