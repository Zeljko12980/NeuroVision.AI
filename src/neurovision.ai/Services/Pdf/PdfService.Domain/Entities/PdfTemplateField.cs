namespace PdfService.Domain.Entities;

public sealed class PdfTemplateField
{
    public Guid Id { get; private set; }

    public Guid PdfTemplateId { get; private set; }

    public PdfTemplate PdfTemplate { get; private set; } = default!;

    public string Name { get; private set; } = string.Empty;

    public string Type { get; private set; } = string.Empty;

    public int Page { get; private set; }

    public float X { get; private set; }

    public float Y { get; private set; }

    public float Width { get; private set; }

    public float Height { get; private set; }

    private PdfTemplateField()
    {
    }

    public static PdfTemplateField Create(
        string name,
        string type,
        int page = 0,
        float x = 0,
        float y = 0,
        float width = 0,
        float height = 0,
        Guid? id = null,
        Guid? pdfTemplateId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Field name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Field type is required.", nameof(type));

        return new PdfTemplateField
        {
            Id = id ?? Guid.NewGuid(),
            PdfTemplateId = pdfTemplateId ?? Guid.Empty,
            Name = name.Trim(),
            Type = type.Trim(),
            Page = page,
            X = x,
            Y = y,
            Width = width,
            Height = height
        };
    }

    public static PdfTemplateField Restore(
        Guid id,
        Guid pdfTemplateId,
        string name,
        string type,
        int page,
        float x,
        float y,
        float width,
        float height)
        => new()
        {
            Id = id,
            PdfTemplateId = pdfTemplateId,
            Name = name,
            Type = type,
            Page = page,
            X = x,
            Y = y,
            Width = width,
            Height = height
        };

    internal void AttachTo(Guid pdfTemplateId)
    {
        PdfTemplateId = pdfTemplateId;
    }
}
