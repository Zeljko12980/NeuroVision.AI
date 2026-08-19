namespace LocationService.Domain.Entities;

public class GovernmentHistory
{
    public string CountryCode { get; set; } = null!;      
    public int SequenceNumber { get; set; }                 
    public string GovernmentTypeCode { get; set; } = null!; 
    public DateTime From { get; set; }                       
    public DateTime? To { get; set; }                        

    public Country Country { get; set; } = null!;
    public GovernmentType GovernmentType { get; set; } = null!;
}
