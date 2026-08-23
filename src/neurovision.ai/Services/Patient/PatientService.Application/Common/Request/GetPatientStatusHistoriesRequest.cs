namespace PatientService.Application.Common.Request;

public record GetPatientStatusHistoriesRequest(string? Search) : PaginationRequest;
