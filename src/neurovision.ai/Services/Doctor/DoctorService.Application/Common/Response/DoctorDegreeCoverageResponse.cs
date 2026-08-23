namespace DoctorService.Application.Common.Response;

public class DoctorDegreeCoverageResponse
{
    public Guid DoctorId { get; set; }
    public string DegreeTypeCode { get; set; } = null!;
    public string? InstitutionName { get; set; }
    public int? Year { get; set; }
}
