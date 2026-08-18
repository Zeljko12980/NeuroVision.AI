namespace MailService.Domain.Templates;

public sealed class EmailTemplate
{
    private EmailTemplate(
        string id,
        string documentTemplateCode,
        string subject,
        string htmlBody,
        string attachmentFileName,
        IReadOnlyList<string> requiredPlaceholders)
    {
        Id = id;
        DocumentTemplateCode = documentTemplateCode;
        Subject = subject;
        HtmlBody = htmlBody;
        AttachmentFileName = attachmentFileName;
        RequiredPlaceholders = requiredPlaceholders;
    }

    public string Id { get; }
    public string DocumentTemplateCode { get; }
    public string Subject { get; }
    public string HtmlBody { get; }
    public string AttachmentFileName { get; }
    public IReadOnlyList<string> RequiredPlaceholders { get; }

    public static EmailTemplate Create(
        string id,
        string documentTemplateCode,
        string subject,
        string htmlBody,
        string attachmentFileName,
        params string[] requiredPlaceholders)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(documentTemplateCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        ArgumentException.ThrowIfNullOrWhiteSpace(htmlBody);
        ArgumentException.ThrowIfNullOrWhiteSpace(attachmentFileName);
        ArgumentNullException.ThrowIfNull(requiredPlaceholders);

        return new EmailTemplate(
            id,
            documentTemplateCode,
            subject,
            htmlBody,
            attachmentFileName,
            requiredPlaceholders);
    }

    public void EnsurePlaceholders(IReadOnlyDictionary<string, string> placeholders)
    {
        ArgumentNullException.ThrowIfNull(placeholders);

        foreach (var key in RequiredPlaceholders)
        {
            if (!placeholders.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    $"Placeholder '{key}' is required for template '{Id}'.",
                    nameof(placeholders));
            }
        }
    }
}
