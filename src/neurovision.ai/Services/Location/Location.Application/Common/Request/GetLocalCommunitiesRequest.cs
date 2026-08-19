using BuildingBlocks.Pagination;

namespace LocationService.Application.Common.Request
{
    public record GetLocalCommunitiesRequest(string? Search) : PaginationRequest;
}
