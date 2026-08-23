namespace PatientService.Application.Common.Request;

public record GetPatientInsuranceHistoriesRequest(string? Search) : PaginationRequest;
