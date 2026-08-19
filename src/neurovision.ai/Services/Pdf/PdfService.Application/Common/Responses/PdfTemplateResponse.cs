namespace PdfService.Application.Common.Responses;

public class PdfTemplateResponse
{
    public Guid Id { get; init; }

    public required string Code { get; init; }

    public required string Name { get; init; }

    public required string HtmlContent { get; init; }

    public int Version { get; init; }

    public bool IsActive { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? UpdatedAt { get; init; }

    public bool RequiresSignature { get; init; }

    public int SignaturePage { get; init; }

    public List<PdfTemplateFieldResponse> Fields { get; init; } = [];
}
