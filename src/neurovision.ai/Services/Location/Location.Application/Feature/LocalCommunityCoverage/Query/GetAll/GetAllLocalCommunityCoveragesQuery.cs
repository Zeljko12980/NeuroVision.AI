using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LocalCommunityCoverage.Query.GetAll
{
    public sealed record GetAllLocalCommunityCoveragesQuery(GetLocalCommunityCoveragesRequest Request) : IQuery<Result<PaginatedResult<LocalCommunityCoverageResponse>>>;
}
