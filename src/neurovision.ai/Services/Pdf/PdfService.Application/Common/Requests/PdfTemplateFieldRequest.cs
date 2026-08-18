namespace PdfService.Application.Common.Requests;

public class PdfTemplateFieldRequest
{
    public required string Name { get; set; }

    public required string Type { get; set; }

    public int Page { get; set; }

    public float X { get; set; }

    public float Y { get; set; }

    public float Width { get; set; }

    public float Height { get; set; }
}
