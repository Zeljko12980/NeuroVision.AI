namespace DoctorService.Application.Common.Request;

public record GetDoctorAffiliationHistoriesRequest(string? Search) : PaginationRequest;
