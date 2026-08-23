namespace PatientService.Application.Common.Request;

public record GetStatusesRequest(string? Search) : PaginationRequest;
