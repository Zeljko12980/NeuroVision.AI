namespace PatientService.Domain.Entities;

public class RelationshipType
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public ICollection<PatientEmergencyContact> Contacts { get; private set; } = new List<PatientEmergencyContact>();

    private RelationshipType()
    {
    }

    public static RelationshipType Create(string code, string name, string? description = null)
    {
        return new RelationshipType
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
