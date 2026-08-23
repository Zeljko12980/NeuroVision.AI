namespace DoctorService.Application.Common.Request;

public record GetDegreeTypesRequest(string? Search) : PaginationRequest;
