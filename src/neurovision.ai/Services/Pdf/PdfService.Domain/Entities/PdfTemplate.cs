namespace PdfService.Domain.Entities;

public sealed class PdfTemplate
{
    public const string SignatureFieldType = "Signature";
    public const string SignaturePlaceholder = "{{Signature}}";

    private const float DefaultSignatureX = 72f;
    private const float DefaultSignatureY = 72f;
    private const float DefaultSignatureWidth = 150f;
    private const float DefaultSignatureHeight = 50f;

    public Guid Id { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string HtmlContent { get; private set; } = string.Empty;

    public int Version { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public bool RequiresSignature { get; private set; }

    public int SignaturePage { get; private set; }

    public ICollection<PdfTemplateField> Fields { get; private set; } = new List<PdfTemplateField>();

    private PdfTemplate()
    {
    }

    public static PdfTemplate Create(
        string code,
        string name,
        string htmlContent,
        int version = 1,
        bool isActive = true,
        bool requiresSignature = false,
        int signaturePage = 1,
        Guid? id = null,
        IEnumerable<PdfTemplateField>? fields = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Template code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Template name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(htmlContent))
            throw new ArgumentException("Template HTML content is required.", nameof(htmlContent));

        if (version <= 0)
            throw new ArgumentException("Template version must be greater than zero.", nameof(version));

        if (signaturePage < 0)
            throw new ArgumentException("Signature page cannot be negative.", nameof(signaturePage));

        var template = new PdfTemplate
        {
            Id = id ?? Guid.NewGuid(),
            Code = code.Trim(),
            Name = name.Trim(),
            HtmlContent = htmlContent,
            Version = version,
            IsActive = isActive,
            RequiresSignature = requiresSignature,
            SignaturePage = signaturePage == 0 ? 1 : signaturePage,
            CreatedAt = DateTimeOffset.UtcNow
        };

        if (fields is not null)
        {
            foreach (var field in fields)
                template.AddField(field);
        }

        template.EnsureSignatureField();
        return template;
    }

    public static PdfTemplate Restore(
        Guid id,
        string code,
        string name,
        string htmlContent,
        int version,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt,
        bool requiresSignature,
        int signaturePage,
        IEnumerable<PdfTemplateField>? fields = null)
    {
        var template = new PdfTemplate
        {
            Id = id,
            Code = code,
            Name = name,
            HtmlContent = htmlContent,
            Version = version,
            IsActive = isActive,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            RequiresSignature = requiresSignature,
            SignaturePage = signaturePage
        };

        if (fields is not null)
        {
            foreach (var field in fields)
                template.Fields.Add(field);
        }

        return template;
    }

    public void Update(string name, string htmlContent, int version, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Template name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(htmlContent))
            throw new ArgumentException("Template HTML content is required.", nameof(htmlContent));

        if (version <= 0)
            throw new ArgumentException("Template version must be greater than zero.", nameof(version));

        Name = name.Trim();
        HtmlContent = htmlContent;
        Version = version;
        IsActive = isActive;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AddField(PdfTemplateField field)
    {
        ArgumentNullException.ThrowIfNull(field);
        field.AttachTo(Id);
        Fields.Add(field);
    }

    public PdfTemplateField? GetSignatureField() =>
        Fields.FirstOrDefault(field =>
            field.Type.Equals(SignatureFieldType, StringComparison.OrdinalIgnoreCase));

    public void EnsureSignatureField()
    {
        if (!RequiresSignature || GetSignatureField() is not null)
            return;

        AddField(PdfTemplateField.Create(
            SignatureFieldType,
            SignatureFieldType,
            page: 0,
            x: DefaultSignatureX,
            y: DefaultSignatureY,
            width: DefaultSignatureWidth,
            height: DefaultSignatureHeight,
            pdfTemplateId: Id));
    }

    public string RenderHtml(
        IReadOnlyDictionary<string, string> data,
        byte[]? signatureImage = null)
    {
        var html = HtmlContent;

        foreach (var item in data)
        {
            var value = item.Value ?? string.Empty;
            var name = NormalizePlaceholderName(item.Key);

            html = html.Replace(
                $"{{{{{name}}}}}",
                value,
                StringComparison.OrdinalIgnoreCase);

            html = html.Replace(
                $"@Model.{name}",
                value,
                StringComparison.OrdinalIgnoreCase);
        }

        if (!html.Contains(SignaturePlaceholder, StringComparison.OrdinalIgnoreCase))
            return html;

        if (signatureImage is not { Length: > 0 })
        {
            return html.Replace(
                SignaturePlaceholder,
                string.Empty,
                StringComparison.OrdinalIgnoreCase);
        }

        var signatureHtml = $"""
            <img src="data:image/png;base64,{Convert.ToBase64String(signatureImage)}" style="max-width:220px;max-height:70px;object-fit:contain;" alt="Physician signature" />
            """;

        return html.Replace(
            SignaturePlaceholder,
            signatureHtml,
            StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePlaceholderName(string key)
    {
        const string modelPrefix = "@Model.";
        return key.StartsWith(modelPrefix, StringComparison.OrdinalIgnoreCase)
            ? key[modelPrefix.Length..]
            : key;
    }
}
