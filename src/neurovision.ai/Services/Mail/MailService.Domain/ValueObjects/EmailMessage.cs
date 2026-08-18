namespace MailService.Domain.ValueObjects;

public sealed class EmailMessage
{
    private EmailMessage(
        EmailAddress to,
        string subject,
        string htmlBody,
        IReadOnlyList<EmailAttachment> attachments)
    {
        To = to;
        Subject = subject;
        HtmlBody = htmlBody;
        Attachments = attachments;
    }

    public EmailAddress To { get; }
    public string Subject { get; }
    public string HtmlBody { get; }
    public IReadOnlyList<EmailAttachment> Attachments { get; }

    public static EmailMessage Create(
        EmailAddress to,
        string subject,
        string htmlBody,
        IReadOnlyList<EmailAttachment>? attachments = null)
    {
        ArgumentNullException.ThrowIfNull(to);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        ArgumentException.ThrowIfNullOrWhiteSpace(htmlBody);

        return new EmailMessage(
            to,
            subject,
            htmlBody,
            attachments ?? Array.Empty<EmailAttachment>());
    }
}
