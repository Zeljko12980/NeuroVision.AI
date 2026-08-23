namespace PatientService.Application.Common.Request;

public sealed class CreateAllergyRequest
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
