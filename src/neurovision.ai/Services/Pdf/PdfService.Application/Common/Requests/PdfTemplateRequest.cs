namespace PdfService.Application.Common.Requests;

public class PdfTemplateRequest
{
    public required string Code { get; set; }

    public required string Name { get; set; }

    public required string HtmlContent { get; set; }

    public int Version { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    public bool RequiresSignature { get; set; }

    public int SignaturePage { get; set; } = 1;

    public List<PdfTemplateFieldRequest> Fields { get; set; } = [];
}
