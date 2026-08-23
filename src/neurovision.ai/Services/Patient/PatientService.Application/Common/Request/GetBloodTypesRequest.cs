namespace PatientService.Application.Common.Request;

public record GetBloodTypesRequest(string? Search) : PaginationRequest;
