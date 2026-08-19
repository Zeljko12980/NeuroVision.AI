namespace LocationService.Domain.Entities;

public class Municipality
{
    public string CountryCode { get; set; } = null!;  
    public int Code { get; set; }                    
    public string Name { get; set; } = null!;           
    public int? SeatSettlementCode { get; set; }          

    public Country Country { get; set; } = null!;
    public Settlement? SeatSettlement { get; set; }

    public ICollection<LocalCommunity> LocalCommunities { get; set; } = new List<LocalCommunity>();
    public ICollection<MunicipalitySettlementCoverage> Settlements { get; set; } = new List<MunicipalitySettlementCoverage>();
}
