namespace PatientService.Application.Common.Request;

public record GetAllergiesRequest(string? Search) : PaginationRequest;
