namespace DoctorService.Application.Common.Request;

public record GetDoctorReviewsRequest(string? Search) : PaginationRequest;
