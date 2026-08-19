namespace LocationService.Domain.Entities;

public class Capital
{
    public string CountryCode { get; set; } = null!;  
    public int SettlementCode { get; set; }             
    public int SequenceNumber { get; set; }               
    public DateTime From { get; set; }                   
    public DateTime? To { get; set; }                      
    public Country Country { get; set; } = null!;
    public Settlement Settlement { get; set; } = null!;
}
