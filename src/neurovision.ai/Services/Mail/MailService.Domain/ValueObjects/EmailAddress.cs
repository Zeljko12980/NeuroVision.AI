using System.Text.RegularExpressions;

namespace MailService.Domain.ValueObjects;

public sealed class EmailAddress : IEquatable<EmailAddress>
{
    private static readonly Regex Format = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private EmailAddress(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static EmailAddress Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var normalized = value.Trim().ToLowerInvariant();
        if (!IsValid(normalized))
            throw new ArgumentException("Invalid email address.", nameof(value));

        return new EmailAddress(normalized);
    }

    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return Format.IsMatch(value.Trim());
    }

    public bool Equals(EmailAddress? other) => other is not null && Value == other.Value;

    public override bool Equals(object? obj) => obj is EmailAddress other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
