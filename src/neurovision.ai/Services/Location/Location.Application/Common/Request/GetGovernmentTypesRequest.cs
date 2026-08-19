using BuildingBlocks.Pagination;

namespace LocationService.Application.Common.Request
{
    public record GetGovernmentTypesRequest(string? Search) : PaginationRequest;
}
