namespace PatientService.Application.Common.Request;

public record GetGendersRequest(string? Search) : PaginationRequest;
