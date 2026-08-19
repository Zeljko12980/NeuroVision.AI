using BuildingBlocks.Pagination;

namespace LocationService.Application.Common.Request
{
    public record GetHealthInstitutionTypesRequest(string? Search) : PaginationRequest;
}
