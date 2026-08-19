namespace LocationService.Domain.Entities;

public class LocalCommunityCoverage
{
    public string CountryCode { get; private set; } = null!;
    public int MunicipalityCode { get; private set; }
    public int LocalCommunityIdentifier { get; private set; }
    public int SettlementCode { get; private set; }

    public LocalCommunity LocalCommunity { get; private set; } = null!;
    public Settlement Settlement { get; private set; } = null!;

    private LocalCommunityCoverage()
    {
    }

    public static LocalCommunityCoverage Create(
        string countryCode,
        int municipalityCode,
        int localCommunityIdentifier,
        int settlementCode)
    {
        if (municipalityCode <= 0)
            throw new ArgumentException("Municipality code must be greater than zero.", nameof(municipalityCode));

        if (localCommunityIdentifier <= 0)
            throw new ArgumentException("Local community identifier must be greater than zero.", nameof(localCommunityIdentifier));

        if (settlementCode <= 0)
            throw new ArgumentException("Settlement code must be greater than zero.", nameof(settlementCode));

        return new LocalCommunityCoverage
        {
            CountryCode = Guard.NotEmpty(countryCode, nameof(countryCode)).ToUpperInvariant(),
            MunicipalityCode = municipalityCode,
            LocalCommunityIdentifier = localCommunityIdentifier,
            SettlementCode = settlementCode
        };
    }
}
