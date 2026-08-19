namespace PdfService.Application.Common.Requests;

public class GeneratePdfRequest
{
    public string TemplateCode { get; set; } = string.Empty;

    public Dictionary<string, string> Data { get; set; } = [];

    public Guid? CertificateId { get; set; }

    public Guid? UserId { get; set; }
}
