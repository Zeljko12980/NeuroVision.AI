namespace PatientService.Domain.Entities;

public class InsurancePayer
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public ICollection<PatientInsuranceHistory> Histories { get; private set; } = new List<PatientInsuranceHistory>();

    private InsurancePayer()
    {
    }

    public static InsurancePayer Create(string code, string name, string? description = null)
    {
        return new InsurancePayer
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
