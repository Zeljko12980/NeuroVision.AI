namespace AppointmentService.Domain.Entities;

public class AppointmentStatus
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();

    private AppointmentStatus()
    {
    }

    public static AppointmentStatus Create(string code, string name, string? description = null)
    {
        return new AppointmentStatus
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
