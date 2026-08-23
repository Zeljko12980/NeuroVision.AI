namespace PatientService.Application.Common.Request;

public record GetLanguagesRequest(string? Search) : PaginationRequest;
