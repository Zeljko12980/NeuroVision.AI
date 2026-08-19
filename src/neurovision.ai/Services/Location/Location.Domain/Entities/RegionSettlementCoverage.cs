namespace LocationService.Domain.Entities;

public class RegionSettlementCoverage
{
    public string RegionTypeCode { get; set; } = null!;  
    public short RegionCode { get; set; }                 
    public string CountryCode { get; set; } = null!;       
    public int SettlementCode { get; set; }                  

    public Region Region { get; set; } = null!;
    public Settlement Settlement { get; set; } = null!;
}
