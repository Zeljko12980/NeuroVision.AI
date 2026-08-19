using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LocalCommunityCoverage.Query.GetAll
{
    public sealed class GetAllLocalCommunityCoveragesQueryHandler : IQueryHandler<GetAllLocalCommunityCoveragesQuery, Result<PaginatedResult<LocalCommunityCoverageResponse>>>
    {
        private readonly ILocalCommunityCoverageService _service;

        public GetAllLocalCommunityCoveragesQueryHandler(ILocalCommunityCoverageService service)
        {
            _service = service;
        }

        public async Task<Result<PaginatedResult<LocalCommunityCoverageResponse>>> Handle(GetAllLocalCommunityCoveragesQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
