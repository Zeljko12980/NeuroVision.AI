using BuildingBlocks.Pagination;

namespace DoctorService.Application.Common.Request;

public record GetDoctorsRequest(string? Search) : PaginationRequest;
