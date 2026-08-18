using System.Text.RegularExpressions;

namespace IdentityService.Domain.Constants;

public static class InternationalPhoneNumber
{
    public const string Example = "+387 61 123 456";

    // E.164: '+' then 8–15 digits, first digit 1–9.
    private const string CompactPattern = @"^\+[1-9]\d{7,14}$";

    public static string? Normalize(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return null;

        var compact = Regex.Replace(phoneNumber.Trim(), @"[\s\-()]", string.Empty);
        return string.IsNullOrWhiteSpace(compact) ? null : compact;
    }

    public static bool IsValid(string? phoneNumber)
    {
        var normalized = Normalize(phoneNumber);
        return normalized is null || Regex.IsMatch(normalized, CompactPattern);
    }
}
