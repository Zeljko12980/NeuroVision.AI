namespace LocationService.Domain.Entities;

public class RegionType
{
    public string Code { get; set; } = null!; 
    public string Name { get; set; } = null!;  
    public string? Description { get; set; }  

    public ICollection<Region> Regions { get; set; } = new List<Region>();
}
