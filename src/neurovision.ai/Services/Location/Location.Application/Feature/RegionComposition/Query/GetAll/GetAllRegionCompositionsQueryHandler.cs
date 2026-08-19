using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionComposition.Query.GetAll
{
    public sealed class GetAllRegionCompositionsQueryHandler : IQueryHandler<GetAllRegionCompositionsQuery, Result<PaginatedResult<RegionCompositionResponse>>>
    {
        private readonly IRegionCompositionService _service;

        public GetAllRegionCompositionsQueryHandler(IRegionCompositionService service)
        {
            _service = service;
        }

        public async Task<Result<PaginatedResult<RegionCompositionResponse>>> Handle(GetAllRegionCompositionsQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
