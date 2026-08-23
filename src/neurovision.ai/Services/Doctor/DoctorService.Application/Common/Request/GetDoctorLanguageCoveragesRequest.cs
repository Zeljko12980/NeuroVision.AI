namespace DoctorService.Application.Common.Request;

public record GetDoctorLanguageCoveragesRequest(string? Search) : PaginationRequest;
