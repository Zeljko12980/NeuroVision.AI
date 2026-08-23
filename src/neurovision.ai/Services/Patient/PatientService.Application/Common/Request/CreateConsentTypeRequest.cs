namespace PatientService.Application.Common.Request;

public sealed class CreateConsentTypeRequest
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
