namespace DoctorService.Application.Common.Request;

public record GetDoctorDegreeCoveragesRequest(string? Search) : PaginationRequest;
