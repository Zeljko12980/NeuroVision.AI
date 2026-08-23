namespace DoctorService.Application.Common.Request;

public record GetDoctorStatusHistoriesRequest(string? Search) : PaginationRequest;
