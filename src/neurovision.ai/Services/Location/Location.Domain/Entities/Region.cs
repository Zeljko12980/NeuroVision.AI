namespace LocationService.Domain.Entities;

public class Region
{
    public string TypeCode { get; private set; } = null!;
    public short Code { get; private set; }
    public string Name { get; private set; } = null!;
    public string? BelongsToCountryCode { get; private set; }
    public string? HeadquartersCountryCode { get; private set; }
    public int? AdministrativeSeatSettlementCode { get; private set; }

    public RegionType Type { get; private set; } = null!;
    public Country? BelongsToCountry { get; private set; }
    public Settlement? AdministrativeSeatSettlement { get; private set; }

    public ICollection<RegionSettlementCoverage> SettlementCoverages { get; private set; } = new List<RegionSettlementCoverage>();
    public ICollection<RegionComposition> AsParentOf { get; private set; } = new List<RegionComposition>();
    public ICollection<RegionComposition> AsMemberOf { get; private set; } = new List<RegionComposition>();

    private Region()
    {
    }

    public static Region Create(
        string typeCode,
        short code,
        string name,
        string? belongsToCountryCode = null,
        string? headquartersCountryCode = null,
        int? administrativeSeatSettlementCode = null)
    {
        if (code <= 0)
            throw new ArgumentException("Region code must be greater than zero.", nameof(code));

        return new Region
        {
            TypeCode = Guard.NotEmpty(typeCode, nameof(typeCode)),
            Code = code,
            Name = Guard.NotEmpty(name, nameof(name)),
            BelongsToCountryCode = string.IsNullOrWhiteSpace(belongsToCountryCode) ? null : belongsToCountryCode.Trim(),
            HeadquartersCountryCode = string.IsNullOrWhiteSpace(headquartersCountryCode) ? null : headquartersCountryCode.Trim(),
            AdministrativeSeatSettlementCode = administrativeSeatSettlementCode
        };
    }

    public void Update(
        string name,
        string? belongsToCountryCode,
        string? headquartersCountryCode,
        int? administrativeSeatSettlementCode)
    {
        Name = Guard.NotEmpty(name, nameof(name));
        BelongsToCountryCode = string.IsNullOrWhiteSpace(belongsToCountryCode) ? null : belongsToCountryCode.Trim();
        HeadquartersCountryCode = string.IsNullOrWhiteSpace(headquartersCountryCode) ? null : headquartersCountryCode.Trim();
        AdministrativeSeatSettlementCode = administrativeSeatSettlementCode;
    }
}
