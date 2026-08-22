namespace DoctorService.Domain.Entities;

public class DegreeType
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public ICollection<DoctorDegreeCoverage> Coverages { get; private set; } = new List<DoctorDegreeCoverage>();

    private DegreeType()
    {
    }

    public static DegreeType Create(string code, string name, string? description = null)
    {
        return new DegreeType
        {
            Code = Guard.Code(code, nameof(code)),
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
