namespace NotificationService.Domain;

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

public static class NotificationTypeCodes
{
    public const string Tumor = "TUMOR";
    public const string Lab = "LAB";
    public const string Medication = "MED";
    public const string Security = "SEC";
    public const string System = "SYS";
    public const string Radiology = "RAD";
    public const string Appointment = "APPT";

    public static readonly string[] All =
    [
        Tumor, Lab, Medication, Security, System, Radiology, Appointment
    ];
}

public static class NotificationSeverityCodes
{
    public const string Critical = "CRIT";
    public const string Warning = "WARN";
    public const string Info = "INFO";
}

public static class NotificationChannelCodes
{
    public const string InApp = "INAPP";
    public const string Email = "EMAIL";

    public static readonly string[] All = [InApp, Email];
}
