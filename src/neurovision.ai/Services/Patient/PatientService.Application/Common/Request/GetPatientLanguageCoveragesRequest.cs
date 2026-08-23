namespace PatientService.Application.Common.Request;

public record GetPatientLanguageCoveragesRequest(string? Search) : PaginationRequest;
