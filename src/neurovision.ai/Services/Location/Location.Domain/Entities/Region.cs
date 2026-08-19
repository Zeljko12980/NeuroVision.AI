namespace LocationService.Domain.Entities;

public class Region
{
    public string TypeCode { get; set; } = null!;              
    public short Code { get; set; }                             
    public string Name { get; set; } = null!;                    
    public string? BelongsToCountryCode { get; set; }            
    public string? HeadquartersCountryCode { get; set; }         
    public int? AdministrativeSeatSettlementCode { get; set; }   

    public RegionType Type { get; set; } = null!;
    public Country? BelongsToCountry { get; set; }
    public Settlement? AdministrativeSeatSettlement { get; set; }

    public ICollection<RegionSettlementCoverage> SettlementCoverages { get; set; } = new List<RegionSettlementCoverage>();
    public ICollection<RegionComposition> AsParentOf { get; set; } = new List<RegionComposition>(); 
    public ICollection<RegionComposition> AsMemberOf { get; set; } = new List<RegionComposition>();  
}
