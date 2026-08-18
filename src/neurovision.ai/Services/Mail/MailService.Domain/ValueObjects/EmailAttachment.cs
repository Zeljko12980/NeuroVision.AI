namespace MailService.Domain.ValueObjects;

public sealed class EmailAttachment
{
    public const string PdfContentType = "application/pdf";

    private EmailAttachment(string fileName, byte[] content, string contentType)
    {
        FileName = fileName;
        Content = content;
        ContentType = contentType;
    }

    public string FileName { get; }
    public byte[] Content { get; }
    public string ContentType { get; }

    public static EmailAttachment Create(string fileName, byte[] content, string contentType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
        ArgumentNullException.ThrowIfNull(content);

        if (content.Length == 0)
            throw new ArgumentException("Attachment content cannot be empty.", nameof(content));

        return new EmailAttachment(fileName, content, contentType);
    }

    public static EmailAttachment Pdf(string fileName, byte[] content)
        => Create(fileName, content, PdfContentType);
}
