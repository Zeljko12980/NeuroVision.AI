namespace LocationService.Domain;

public static class Guard
{
    public static string NotEmpty(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{name} is required.", name);

        return value.Trim();
    }
}

public static class DateRange
{
    public static void EnsureValid(DateTime from, DateTime? to)
    {
        if (to.HasValue && to.Value < from)
            throw new ArgumentException("Period end cannot be before start.");
    }

    public static bool Overlaps(DateTime fromA, DateTime? toA, DateTime fromB, DateTime? toB)
    {
        var endA = toA ?? DateTime.MaxValue;
        var endB = toB ?? DateTime.MaxValue;
        return fromA < endB && fromB < endA;
    }
}
