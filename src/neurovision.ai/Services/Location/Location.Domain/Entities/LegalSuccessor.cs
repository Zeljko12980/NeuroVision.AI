namespace LocationService.Domain.Entities;

public class LegalSuccessor
{
    public string SuccessorCountryCode { get; set; } = null!;    
    public string PredecessorCountryCode { get; set; } = null!;   

    public Country SuccessorCountry { get; set; } = null!;
    public Country PredecessorCountry { get; set; } = null!;
}
