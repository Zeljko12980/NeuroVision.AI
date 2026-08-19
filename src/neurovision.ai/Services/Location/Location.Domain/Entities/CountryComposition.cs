namespace LocationService.Domain.Entities;

public class CountryComposition
{
    public string UnionCountryCode { get; set; } = null!; 
    public string MemberCountryCode { get; set; } = null!; 
    public int SequenceNumber { get; set; }                    
    public DateTime From { get; set; }                      
    public DateTime? To { get; set; }                       

    public Country UnionCountry { get; set; } = null!;
    public Country MemberCountry { get; set; } = null!;
}
