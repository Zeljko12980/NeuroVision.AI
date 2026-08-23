namespace DoctorService.Application.Common.Request;

public sealed class CreateDoctorDegreeCoverageRequest
{
    public Guid DoctorId { get; set; }
    public string DegreeTypeCode { get; set; } = null!;
    public string? InstitutionName { get; set; }
    public int? Year { get; set; }
}
