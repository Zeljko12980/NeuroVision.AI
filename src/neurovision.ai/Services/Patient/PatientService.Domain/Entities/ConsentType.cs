namespace PatientService.Domain.Entities;

public class ConsentType
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public ICollection<PatientConsentCoverage> Coverages { get; private set; } = new List<PatientConsentCoverage>();

    private ConsentType()
    {
    }

    public static ConsentType Create(string code, string name, string? description = null)
    {
        return new ConsentType
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
