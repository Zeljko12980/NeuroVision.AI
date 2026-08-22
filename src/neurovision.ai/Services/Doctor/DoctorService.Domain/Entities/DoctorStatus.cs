namespace DoctorService.Domain.Entities;

public class DoctorStatus
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public ICollection<Doctor> Doctors { get; private set; } = new List<Doctor>();
    public ICollection<DoctorStatusHistory> Histories { get; private set; } = new List<DoctorStatusHistory>();

    private DoctorStatus()
    {
    }

    public static DoctorStatus Create(string code, string name, string? description = null)
    {
        return new DoctorStatus
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
