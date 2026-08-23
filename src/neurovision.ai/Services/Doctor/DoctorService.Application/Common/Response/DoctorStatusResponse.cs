namespace DoctorService.Application.Common.Response;

public class DoctorStatusResponse
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
