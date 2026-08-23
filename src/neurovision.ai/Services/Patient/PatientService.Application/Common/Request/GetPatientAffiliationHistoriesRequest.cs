namespace PatientService.Application.Common.Request;

public record GetPatientAffiliationHistoriesRequest(string? Search) : PaginationRequest;
