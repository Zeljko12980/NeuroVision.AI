namespace PatientService.Application.Common.Response;

public class PatientAllergyCoverageResponse
{
    public Guid PatientId { get; set; }
    public string AllergyCode { get; set; } = null!;
    public string? Note { get; set; }
}
