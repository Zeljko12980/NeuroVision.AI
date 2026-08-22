namespace DoctorService.Application.Common.Response;

public class LicenseAuthorityResponse
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
