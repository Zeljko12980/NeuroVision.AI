namespace LocationService.Domain.Entities;

public class LocalCommunity
{
    public string CountryCode { get; set; } = null!;     
    public int MunicipalityCode { get; set; }               
    public int Identifier { get; set; }                     
    public string Name { get; set; } = null!;             
    public int? OfficeSettlementCode { get; set; }         

    public Municipality Municipality { get; set; } = null!;
    public Settlement? OfficeSettlement { get; set; }

    public ICollection<LocalCommunityCoverage> Coverages { get; set; } = new List<LocalCommunityCoverage>();
}
