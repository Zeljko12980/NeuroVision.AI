namespace DoctorService.Domain.Entities;

public class LicenseAuthority
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public ICollection<Doctor> Doctors { get; private set; } = new List<Doctor>();
    public ICollection<DoctorLicenseHistory> Histories { get; private set; } = new List<DoctorLicenseHistory>();

    private LicenseAuthority()
    {
    }

    public static LicenseAuthority Create(string code, string name, string? description = null)
    {
        return new LicenseAuthority
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
