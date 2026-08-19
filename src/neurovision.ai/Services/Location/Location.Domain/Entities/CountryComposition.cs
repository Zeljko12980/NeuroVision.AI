namespace LocationService.Domain.Entities;

public class CountryComposition
{
    public string UnionCountryCode { get; private set; } = null!;
    public string MemberCountryCode { get; private set; } = null!;
    public int SequenceNumber { get; private set; }
    public DateTime From { get; private set; }
    public DateTime? To { get; private set; }

    public Country UnionCountry { get; private set; } = null!;
    public Country MemberCountry { get; private set; } = null!;

    private CountryComposition()
    {
    }

    public static CountryComposition Create(
        string unionCountryCode,
        string memberCountryCode,
        int sequenceNumber,
        DateTime from,
        DateTime? to = null)
    {
        if (sequenceNumber <= 0)
            throw new ArgumentException("Sequence number must be greater than zero.", nameof(sequenceNumber));

        DateRange.EnsureValid(from, to);

        var union = Guard.NotEmpty(unionCountryCode, nameof(unionCountryCode)).ToUpperInvariant();
        var member = Guard.NotEmpty(memberCountryCode, nameof(memberCountryCode)).ToUpperInvariant();

        if (union == member)
            throw new ArgumentException("Union and member countries must be different.");

        return new CountryComposition
        {
            UnionCountryCode = union,
            MemberCountryCode = member,
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
