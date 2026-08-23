namespace PatientService.Application.Common.Request;

public record GetConditionsRequest(string? Search) : PaginationRequest;
