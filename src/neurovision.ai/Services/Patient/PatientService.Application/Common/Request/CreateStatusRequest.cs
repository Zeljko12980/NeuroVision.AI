namespace PatientService.Application.Common.Request;

public sealed class CreateStatusRequest
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
