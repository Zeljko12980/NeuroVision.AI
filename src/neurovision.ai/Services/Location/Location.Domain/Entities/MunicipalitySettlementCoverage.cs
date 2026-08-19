namespace LocationService.Domain.Entities;


public class MunicipalitySettlementCoverage
{
    public string CountryCode { get; set; } = null!;  
    public int MunicipalityCode { get; set; }           
    public int SettlementCode { get; set; }              

    public Municipality Municipality { get; set; } = null!;
    public Settlement Settlement { get; set; } = null!;
}
