namespace DoctorService.Domain.Entities;

public class Language
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public ICollection<DoctorLanguageCoverage> Coverages { get; private set; } = new List<DoctorLanguageCoverage>();

    private Language()
    {
    }

    public static Language Create(string code, string name, string? description = null)
    {
        return new Language
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
