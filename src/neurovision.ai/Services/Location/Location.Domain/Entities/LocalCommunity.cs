namespace LocationService.Domain.Entities;

public class LocalCommunity
{
    public string CountryCode { get; private set; } = null!;
    public int MunicipalityCode { get; private set; }
    public int Identifier { get; private set; }
    public string Name { get; private set; } = null!;
    public int? OfficeSettlementCode { get; private set; }

    public Municipality Municipality { get; private set; } = null!;
    public Settlement? OfficeSettlement { get; private set; }

    public ICollection<LocalCommunityCoverage> Coverages { get; private set; } = new List<LocalCommunityCoverage>();

    private LocalCommunity()
    {
    }

    public static LocalCommunity Create(
        string countryCode,
        int municipalityCode,
        int identifier,
        string name,
        int? officeSettlementCode = null)
    {
        if (municipalityCode <= 0)
            throw new ArgumentException("Municipality code must be greater than zero.", nameof(municipalityCode));

        if (identifier <= 0)
            throw new ArgumentException("Local community identifier must be greater than zero.", nameof(identifier));

        return new LocalCommunity
        {
            CountryCode = Guard.NotEmpty(countryCode, nameof(countryCode)).ToUpperInvariant(),
            MunicipalityCode = municipalityCode,
            Identifier = identifier,
            Name = Guard.NotEmpty(name, nameof(name)),
            OfficeSettlementCode = officeSettlementCode
        };
    }

    public void Update(string name, int? officeSettlementCode)
    {
        Name = Guard.NotEmpty(name, nameof(name));
        OfficeSettlementCode = officeSettlementCode;
    }
}
