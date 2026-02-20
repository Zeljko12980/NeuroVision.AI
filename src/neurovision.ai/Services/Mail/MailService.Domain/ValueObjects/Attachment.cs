namespace MailService.Domain.ValueObjects
{
    public sealed class Attachment
    {
        public string FileName { get; }
        public byte[] Content { get; }
        public string ContentType { get; }
        public long SizeInBytes { get; }

        private Attachment(string fileName, byte[] content, string contentType)
        {
            FileName = fileName;
            Content = content;
            ContentType = contentType;
            SizeInBytes = content.Length;
        }

        public static Attachment Create(string fileName, byte[] content, string contentType)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Ime fajla ne može biti prazno.", nameof(fileName));

            if (content == null || content.Length == 0)
                throw new ArgumentException("Sadržaj attachmenta ne može biti prazan.", nameof(content));

            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("Content type ne može biti prazan.", nameof(contentType));

            const long maxSize = 25 * 1024 * 1024;
            if (content.Length > maxSize)
                throw new ArgumentException($"Attachment ne može biti veći od 25MB. Trenutna veličina: {content.Length / (1024.0 * 1024.0):F2}MB");

            return new Attachment(fileName, content, contentType);
        }

        public double SizeInMB => SizeInBytes / (1024.0 * 1024.0);
    }
}
