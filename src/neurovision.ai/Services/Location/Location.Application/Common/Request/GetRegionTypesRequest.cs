using BuildingBlocks.Pagination;

namespace LocationService.Application.Common.Request
{
    public record GetRegionTypesRequest(string? Search) : PaginationRequest;
}
