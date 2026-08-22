namespace PatientService.Domain;

public static class Guard
{
    public static string NotEmpty(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{name} is required.", name);

        return value.Trim();
    }

    public static string Code(string? value, string name, int maxLength = 10)
    {
        var code = NotEmpty(value, name).ToUpperInvariant();
        if (code.Length > maxLength)
            throw new ArgumentException($"{name} cannot exceed {maxLength} characters.", name);

        return code;
    }
}

public static class DateRange
{
    public static void EnsureValid(DateTime from, DateTime? to)
    {
        if (to.HasValue && to.Value < from)
            throw new ArgumentException("Period end cannot be before start.");
    }
}

public static class PatientStatusCodes
{
    public const string PendingVerification = "PEND";
    public const string Active = "ACT";
    public const string Inactive = "INAC";
    public const string Deceased = "DECD";
    public const string Archived = "ARCH";
}

public static class GenderCodes
{
    public const string Male = "M";
    public const string Female = "F";
    public const string Other = "O";
    public const string Unknown = "U";
}
