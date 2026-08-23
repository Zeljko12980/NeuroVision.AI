namespace DoctorService.Application.Common.Request;

public record GetDoctorSpecializationCoveragesRequest(string? Search) : PaginationRequest;
