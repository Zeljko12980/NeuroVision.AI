namespace PatientService.Domain.Entities;

public class Condition
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public ICollection<PatientConditionCoverage> Coverages { get; private set; } = new List<PatientConditionCoverage>();

    private Condition()
    {
    }

    public static Condition Create(string code, string name, string? description = null)
    {
        return new Condition
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
