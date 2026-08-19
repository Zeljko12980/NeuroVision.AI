using BuildingBlocks.Pagination;

namespace LocationService.Application.Common.Request
{
    public record GetHealthInstitutionsRequest(string? Search) : PaginationRequest;
}
