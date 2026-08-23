namespace PatientService.Application.Common.Request;

public sealed class CreateLanguageRequest
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
