namespace DoctorService.Application.Common.Request;

public sealed class CreateDegreeTypeRequest
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
