namespace PatientService.Application.Common.Request;

public record GetPatientAllergyCoveragesRequest(string? Search) : PaginationRequest;
