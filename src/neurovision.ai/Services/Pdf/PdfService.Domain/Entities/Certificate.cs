namespace PdfService.Domain.Entities
{
    public sealed class Certificate
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public Guid? UserId { get; private set; }
        public string Subject { get; private set; } = string.Empty;
        public string Issuer { get; private set; } = string.Empty;
        public string Thumbprint { get; private set; } = string.Empty;
        public string SerialNumber { get; private set; } = string.Empty;
        public DateTime ValidFrom { get; private set; }
        public DateTime ValidTo { get; private set; }
        public string FileName { get; private set; } = string.Empty;
        public string FilePath { get; private set; } = string.Empty;
        public string? SignatureImagePath { get; private set; }
        public bool IsDefault { get; private set; }

        public string ProtectedPassword { get; private set; } = string.Empty;

        private Certificate() { }

        public static Certificate Create(
            string name,
            string subject,
            string issuer,
            string thumbprint,
            string serialNumber,
            DateTime validFrom,
            DateTime validTo,
            string fileName,
            string filePath,
            string protectedPassword,
            bool isDefault = false,
            Guid? userId = null,
            string? signatureImagePath = null,
            Guid? id = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Certificate name is required.", nameof(name));

            if (string.IsNullOrWhiteSpace(thumbprint))
                throw new ArgumentException("Certificate thumbprint is required.", nameof(thumbprint));

            if (string.IsNullOrWhiteSpace(protectedPassword))
                throw new ArgumentException("Protected password is required.", nameof(protectedPassword));

            if (validTo <= validFrom)
                throw new ArgumentException("ValidTo must be later than ValidFrom.", nameof(validTo));

            return new Certificate
            {
                Id = id ?? Guid.NewGuid(),
                Name = name,
                UserId = userId,
                Subject = subject,
                Issuer = issuer,
                Thumbprint = thumbprint,
                SerialNumber = serialNumber,
                ValidFrom = validFrom,
                ValidTo = validTo,
                FileName = fileName,
                FilePath = filePath,
                SignatureImagePath = string.IsNullOrWhiteSpace(signatureImagePath)
                    ? null
                    : signatureImagePath,
                ProtectedPassword = protectedPassword,
                IsDefault = isDefault
            };
        }

        public static Certificate Restore(
            Guid id,
            string name,
            string subject,
            string issuer,
            string thumbprint,
            string serialNumber,
            DateTime validFrom,
            DateTime validTo,
            string fileName,
            string filePath,
            string protectedPassword,
            bool isDefault,
            Guid? userId = null,
            string? signatureImagePath = null)
            => new()
            {
                Id = id,
                Name = name,
                UserId = userId,
                Subject = subject,
                Issuer = issuer,
                Thumbprint = thumbprint,
                SerialNumber = serialNumber,
                ValidFrom = validFrom,
                ValidTo = validTo,
                FileName = fileName,
                FilePath = filePath,
                SignatureImagePath = signatureImagePath,
                ProtectedPassword = protectedPassword,
                IsDefault = isDefault
            };

        public void UpdateMetadata(
            string subject,
            string issuer,
            string thumbprint,
            string serialNumber,
            DateTime validFrom,
            DateTime validTo)
        {
            Subject = subject;
            Issuer = issuer;
            Thumbprint = thumbprint;
            SerialNumber = serialNumber;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }

        public void UpdateFilePath(string filePath, string? fileName = null)
        {
            FilePath = filePath;
            if (!string.IsNullOrWhiteSpace(fileName))
                FileName = fileName;
        }

        public void UpdateSignatureImagePath(string? signatureImagePath) =>
            SignatureImagePath = string.IsNullOrWhiteSpace(signatureImagePath)
                ? null
                : signatureImagePath;

        public void UpdateProtectedPassword(string protectedPassword)
        {
            if (string.IsNullOrWhiteSpace(protectedPassword))
                throw new ArgumentException("Protected password is required.", nameof(protectedPassword));

            ProtectedPassword = protectedPassword;
        }

        public bool IsExpired(DateTime? utcNow = null) =>
            ValidTo < (utcNow ?? DateTime.UtcNow);
    }
}
