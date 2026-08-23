namespace DoctorService.Application.Common.Request;

public record GetSpecializationsRequest(string? Search) : PaginationRequest;
