namespace PatientService.Application.Common.Request;

public record GetPatientConditionCoveragesRequest(string? Search) : PaginationRequest;
