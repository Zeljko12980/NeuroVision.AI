namespace PatientService.Domain.Entities;

public class BloodType
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public ICollection<Patient> Patients { get; private set; } = new List<Patient>();

    private BloodType()
    {
    }

    public static BloodType Create(string code, string name, string? description = null)
    {
        return new BloodType
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
