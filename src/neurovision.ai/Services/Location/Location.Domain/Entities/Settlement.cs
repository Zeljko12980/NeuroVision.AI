namespace LocationService.Domain.Entities;

public class Settlement
{
    public string CountryCode { get; set; } = null!;  
    public int Code { get; set; }                      
    public string Name { get; set; } = null!;       
    public string? PostalCode { get; set; }             

    public Country Country { get; set; } = null!;

    public ICollection<Municipality> MunicipalitySeatOf { get; set; } = new List<Municipality>();
    public ICollection<LocalCommunity> LocalCommunityOffices { get; set; } = new List<LocalCommunity>();
    public ICollection<MunicipalitySettlementCoverage> MunicipalityCoverages { get; set; } = new List<MunicipalitySettlementCoverage>();
    public ICollection<LocalCommunityCoverage> LocalCommunityCoverages { get; set; } = new List<LocalCommunityCoverage>();
    public ICollection<RegionSettlementCoverage> RegionCoverages { get; set; } = new List<RegionSettlementCoverage>();
    public ICollection<Capital> CapitalOf { get; set; } = new List<Capital>();
    public ICollection<Region> RegionAdministrativeSeatOf { get; set; } = new List<Region>();
    public ICollection<HealthInstitution> HealthInstitutions { get; set; } = new List<HealthInstitution>();
}
