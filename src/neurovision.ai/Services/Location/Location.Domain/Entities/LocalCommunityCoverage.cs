namespace LocationService.Domain.Entities;

public class LocalCommunityCoverage
{
    public string CountryCode { get; set; } = null!;      
    public int MunicipalityCode { get; set; }            
    public int LocalCommunityIdentifier { get; set; }       
    public int SettlementCode { get; set; }                  

    public LocalCommunity LocalCommunity { get; set; } = null!;
    public Settlement Settlement { get; set; } = null!;
}
