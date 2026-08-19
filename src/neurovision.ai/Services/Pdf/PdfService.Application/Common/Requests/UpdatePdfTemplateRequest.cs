namespace PdfService.Application.Common.Requests;

public class UpdatePdfTemplateRequest
{
    public required string Name { get; set; }

    public required string HtmlContent { get; set; }

    public int Version { get; set; }

    public bool IsActive { get; set; }
}
