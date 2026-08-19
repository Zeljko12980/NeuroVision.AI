using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LocalCommunity.Query.GetAll
{
    public sealed record GetAllLocalCommunitiesQuery(GetLocalCommunitiesRequest Request) : IQuery<Result<PaginatedResult<LocalCommunityResponse>>>;
}
