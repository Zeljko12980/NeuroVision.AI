namespace DoctorService.Application.Common.Request;

public record GetDoctorStatusesRequest(string? Search) : PaginationRequest;
