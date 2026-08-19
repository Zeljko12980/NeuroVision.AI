namespace LocationService.Domain.Entities;

public class GovernmentHistory
{
    public string CountryCode { get; private set; } = null!;
    public int SequenceNumber { get; private set; }
    public string GovernmentTypeCode { get; private set; } = null!;
    public DateTime From { get; private set; }
    public DateTime? To { get; private set; }

    public Country Country { get; private set; } = null!;
    public GovernmentType GovernmentType { get; private set; } = null!;

    private GovernmentHistory()
    {
    }

    public static GovernmentHistory Create(
        string countryCode,
        int sequenceNumber,
        string governmentTypeCode,
        DateTime from,
        DateTime? to = null)
    {
        if (sequenceNumber <= 0)
            throw new ArgumentException("Sequence number must be greater than zero.", nameof(sequenceNumber));

        DateRange.EnsureValid(from, to);

        return new GovernmentHistory
        {
            CountryCode = Guard.NotEmpty(countryCode, nameof(countryCode)).ToUpperInvariant(),
            SequenceNumber = sequenceNumber,
            GovernmentTypeCode = Guard.NotEmpty(governmentTypeCode, nameof(governmentTypeCode)),
            From = from,
            To = to
        };
    }

    public void Update(string governmentTypeCode, DateTime from, DateTime? to)
    {
        DateRange.EnsureValid(from, to);
        GovernmentTypeCode = Guard.NotEmpty(governmentTypeCode, nameof(governmentTypeCode));
        From = from;
        To = to;
    }
}
