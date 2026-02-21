namespace MailService.Domain.ValueObjects
{
    public sealed class EmailAddress : IEquatable<EmailAddress>
    {
        private static readonly Regex EmailRegex = new(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; }

        private EmailAddress(string value)
        {
            Value = value;
        }

        public static EmailAddress Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email ne može biti prazan.", nameof(email));

            email = email.Trim().ToLowerInvariant();

            if (!EmailRegex.IsMatch(email))
                throw new ArgumentException($"Nevažeća email adresa: {email}", nameof(email));

            return new EmailAddress(email);
        }

        public bool Equals(EmailAddress? other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj) => Equals(obj as EmailAddress);

        public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);

        public override string ToString() => Value;

        public static implicit operator string(EmailAddress email) => email.Value;
    }
}
