namespace LocationService.Domain.Entities;

public class GovernmentType
{
    public string Code { get; set; } = null!;  
    public string Name { get; set; } = null!; 
    public string? Description { get; set; }   

    public ICollection<Country> Countries { get; set; } = new List<Country>();
    public ICollection<GovernmentHistory> History { get; set; } = new List<GovernmentHistory>();
}
