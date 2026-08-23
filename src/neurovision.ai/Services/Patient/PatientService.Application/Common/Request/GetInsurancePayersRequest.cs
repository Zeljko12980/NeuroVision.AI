namespace PatientService.Application.Common.Request;

public record GetInsurancePayersRequest(string? Search) : PaginationRequest;
