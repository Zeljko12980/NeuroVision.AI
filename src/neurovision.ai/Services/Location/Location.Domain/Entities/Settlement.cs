namespace LocationService.Domain.Entities;

public class Settlement
{
    public string CountryCode { get; private set; } = null!;
    public int Code { get; private set; }
    public string Name { get; private set; } = null!;
    public string? PostalCode { get; private set; }

    public Country Country { get; private set; } = null!;

    public ICollection<Municipality> MunicipalitySeatOf { get; private set; } = new List<Municipality>();
    public ICollection<LocalCommunity> LocalCommunityOffices { get; private set; } = new List<LocalCommunity>();
    public ICollection<MunicipalitySettlementCoverage> MunicipalityCoverages { get; private set; } = new List<MunicipalitySettlementCoverage>();
    public ICollection<LocalCommunityCoverage> LocalCommunityCoverages { get; private set; } = new List<LocalCommunityCoverage>();
    public ICollection<RegionSettlementCoverage> RegionCoverages { get; private set; } = new List<RegionSettlementCoverage>();
    public ICollection<Capital> CapitalOf { get; private set; } = new List<Capital>();
    public ICollection<Region> RegionAdministrativeSeatOf { get; private set; } = new List<Region>();
    public ICollection<HealthInstitution> HealthInstitutions { get; private set; } = new List<HealthInstitution>();

    private Settlement()
    {
    }

    public static Settlement Create(string countryCode, int code, string name, string? postalCode = null)
    {
        if (code <= 0)
            throw new ArgumentException("Settlement code must be greater than zero.", nameof(code));

        return new Settlement
        {
            CountryCode = Guard.NotEmpty(countryCode, nameof(countryCode)).ToUpperInvariant(),
            Code = code,
            Name = Guard.NotEmpty(name, nameof(name)),
            PostalCode = postalCode
        };
    }

    public void Update(string name, string? postalCode)
    {
        Name = Guard.NotEmpty(name, nameof(name));
        PostalCode = postalCode;
    }
}
