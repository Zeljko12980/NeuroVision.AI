namespace PdfService.Application.Common.Responses;

public class PdfTemplateFieldResponse
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Type { get; init; }

    public int Page { get; init; }

    public float X { get; init; }

    public float Y { get; init; }

    public float Width { get; init; }

    public float Height { get; init; }
}
