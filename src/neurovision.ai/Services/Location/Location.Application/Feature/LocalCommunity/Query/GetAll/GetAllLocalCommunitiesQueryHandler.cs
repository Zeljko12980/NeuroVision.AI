using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LocalCommunity.Query.GetAll
{
    public sealed class GetAllLocalCommunitiesQueryHandler : IQueryHandler<GetAllLocalCommunitiesQuery, Result<PaginatedResult<LocalCommunityResponse>>>
    {
        private readonly ILocalCommunityService _service;

        public GetAllLocalCommunitiesQueryHandler(ILocalCommunityService service)
        {
            _service = service;
        }

        public async Task<Result<PaginatedResult<LocalCommunityResponse>>> Handle(GetAllLocalCommunitiesQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
