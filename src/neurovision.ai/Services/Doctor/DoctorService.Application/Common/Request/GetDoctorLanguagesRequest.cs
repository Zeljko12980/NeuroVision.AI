namespace DoctorService.Application.Common.Request;

public record GetDoctorLanguagesRequest(string? Search) : PaginationRequest;
