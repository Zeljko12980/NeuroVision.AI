using BuildingBlocks.Pagination;

namespace PatientService.Application.Common.Request;

public record GetPatientsRequest(string? Search) : PaginationRequest;
