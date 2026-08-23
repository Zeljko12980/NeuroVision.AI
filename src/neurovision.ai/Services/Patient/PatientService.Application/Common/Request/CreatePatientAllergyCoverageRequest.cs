namespace PatientService.Application.Common.Request;

public sealed class CreatePatientAllergyCoverageRequest
{
    public Guid PatientId { get; set; }
    public string AllergyCode { get; set; } = null!;
    public string? Note { get; set; }
}
