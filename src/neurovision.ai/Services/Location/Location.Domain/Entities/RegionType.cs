namespace LocationService.Domain.Entities;

public class RegionType
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public ICollection<Region> Regions { get; private set; } = new List<Region>();

    private RegionType()
    {
    }

    public static RegionType Create(string code, string name, string? description = null)
    {
        return new RegionType
        {
            Code = Guard.NotEmpty(code, nameof(code)),
            Name = Guard.NotEmpty(name, nameof(name)),
            Description = description
        };
    }

    public void Update(string name, string? description)
    {
        Name = Guard.NotEmpty(name, nameof(name));
        Description = description;
    }
}
