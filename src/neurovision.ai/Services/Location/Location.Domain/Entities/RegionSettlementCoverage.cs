namespace LocationService.Domain.Entities;

public class RegionSettlementCoverage
{
    public string RegionTypeCode { get; private set; } = null!;
    public short RegionCode { get; private set; }
    public string CountryCode { get; private set; } = null!;
    public int SettlementCode { get; private set; }

    public Region Region { get; private set; } = null!;
    public Settlement Settlement { get; private set; } = null!;

    private RegionSettlementCoverage()
    {
    }

    public static RegionSettlementCoverage Create(
        string regionTypeCode,
        short regionCode,
        string countryCode,
        int settlementCode)
    {
        if (regionCode <= 0)
            throw new ArgumentException("Region code must be greater than zero.", nameof(regionCode));

        if (settlementCode <= 0)
            throw new ArgumentException("Settlement code must be greater than zero.", nameof(settlementCode));

        return new RegionSettlementCoverage
        {
            RegionTypeCode = Guard.NotEmpty(regionTypeCode, nameof(regionTypeCode)),
            RegionCode = regionCode,
            CountryCode = Guard.NotEmpty(countryCode, nameof(countryCode)).ToUpperInvariant(),
            SettlementCode = settlementCode
        };
    }
}
