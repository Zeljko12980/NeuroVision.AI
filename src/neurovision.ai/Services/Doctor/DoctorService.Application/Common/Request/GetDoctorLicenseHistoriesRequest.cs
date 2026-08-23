namespace DoctorService.Application.Common.Request;

public record GetDoctorLicenseHistoriesRequest(string? Search) : PaginationRequest;
