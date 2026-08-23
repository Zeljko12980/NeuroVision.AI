namespace PatientService.Application.Common.Request;

public record GetPatientEmergencyContactsRequest(string? Search) : PaginationRequest;
