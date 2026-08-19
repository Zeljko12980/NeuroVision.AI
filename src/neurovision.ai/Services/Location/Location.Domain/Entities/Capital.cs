namespace LocationService.Domain.Entities;

public class Capital
{
    public string CountryCode { get; private set; } = null!;
    public int SettlementCode { get; private set; }
    public int SequenceNumber { get; private set; }
    public DateTime From { get; private set; }
    public DateTime? To { get; private set; }

    public Country Country { get; private set; } = null!;
    public Settlement Settlement { get; private set; } = null!;

    private Capital()
    {
    }

    public static Capital Create(
        string countryCode,
        int settlementCode,
        int sequenceNumber,
        DateTime from,
        DateTime? to = null)
    {
        if (settlementCode <= 0)
            throw new ArgumentException("Settlement code must be greater than zero.", nameof(settlementCode));

        if (sequenceNumber <= 0)
            throw new ArgumentException("Sequence number must be greater than zero.", nameof(sequenceNumber));

        DateRange.EnsureValid(from, to);

        return new Capital
        {
            CountryCode = Guard.NotEmpty(countryCode, nameof(countryCode)).ToUpperInvariant(),
            SettlementCode = settlementCode,
            SequenceNumber = sequenceNumber,
            From = from,
            To = to
        };
    }

    public void Update(DateTime from, DateTime? to)
    {
        DateRange.EnsureValid(from, to);
        From = from;
        To = to;
    }
}
