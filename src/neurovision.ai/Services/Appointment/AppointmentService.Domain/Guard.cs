namespace AppointmentService.Domain;

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

    public static string MaxLength(string value, string name, int maxLength)
    {
        if (value.Length > maxLength)
            throw new ArgumentException($"{name} cannot exceed {maxLength} characters.", name);

        return value;
    }
}

public static class AppointmentTypeCodes
{
    public const string Consultation = "CONS";
    public const string FollowUp = "FUP";
    public const string Scan = "SCAN";

    public static readonly string[] All =
    [
        Consultation, FollowUp, Scan
    ];
}

public static class AppointmentStatusCodes
{
    public const string Scheduled = "SCHD";
    public const string Cancelled = "CANC";
    public const string Completed = "DONE";

    public static readonly string[] All =
    [
        Scheduled, Cancelled, Completed
    ];
}
