using BuildingBlocks.Pagination;

namespace LocationService.Application.Common.Request
{
    public record GetMunicipalitiesRequest(string? Search) : PaginationRequest;
}
