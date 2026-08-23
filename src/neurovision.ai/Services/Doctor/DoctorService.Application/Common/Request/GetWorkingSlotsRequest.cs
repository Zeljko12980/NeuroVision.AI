namespace DoctorService.Application.Common.Request;

public record GetWorkingSlotsRequest(string? Search) : PaginationRequest;
