namespace PatientService.Application.Common.Request;

public record GetPatientConsentCoveragesRequest(string? Search) : PaginationRequest;
