namespace LocationService.Domain.Entities;

public class MunicipalitySettlementCoverage
{
    public string CountryCode { get; private set; } = null!;
    public int MunicipalityCode { get; private set; }
    public int SettlementCode { get; private set; }

    public Municipality Municipality { get; private set; } = null!;
    public Settlement Settlement { get; private set; } = null!;

    private MunicipalitySettlementCoverage()
    {
    }

    public static MunicipalitySettlementCoverage Create(
        string countryCode,
        int municipalityCode,
        int settlementCode)
    {
        if (municipalityCode <= 0)
            throw new ArgumentException("Municipality code must be greater than zero.", nameof(municipalityCode));

        if (settlementCode <= 0)
            throw new ArgumentException("Settlement code must be greater than zero.", nameof(settlementCode));

        return new MunicipalitySettlementCoverage
        {
            CountryCode = Guard.NotEmpty(countryCode, nameof(countryCode)).ToUpperInvariant(),
            MunicipalityCode = municipalityCode,
            SettlementCode = settlementCode
        };
    }
}
