namespace PatientService.Application.Common.Request;

public record GetPatientDoctorAssignmentHistoriesRequest(string? Search) : PaginationRequest;
