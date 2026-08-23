namespace PatientService.Application.Common.Request;

public record GetConsentTypesRequest(string? Search) : PaginationRequest;
