namespace PdfService.Application.Common.Models;

public sealed class SignaturePosition
{
    public int Page { get; init; }

    public float X { get; init; }

    public float Y { get; init; }

    public float Width { get; init; }

    public float Height { get; init; }
}
