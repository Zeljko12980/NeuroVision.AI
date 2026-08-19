using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Region.Query.GetAll
{
    public sealed class GetAllRegionsQueryHandler : IQueryHandler<GetAllRegionsQuery, Result<PaginatedResult<RegionResponse>>>
    {
        private readonly IRegionService _service;

        public GetAllRegionsQueryHandler(IRegionService service)
        {
            _service = service;
        }

        public async Task<Result<PaginatedResult<RegionResponse>>> Handle(GetAllRegionsQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
