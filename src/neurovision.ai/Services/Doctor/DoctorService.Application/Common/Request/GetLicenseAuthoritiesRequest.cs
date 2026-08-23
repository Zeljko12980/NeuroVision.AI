namespace DoctorService.Application.Common.Request;

public record GetLicenseAuthoritiesRequest(string? Search) : PaginationRequest;
