using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionType.Query.GetAll
{
    public sealed class GetAllRegionTypesQueryHandler : IQueryHandler<GetAllRegionTypesQuery, Result<PaginatedResult<RegionTypeResponse>>>
    {
        private readonly IRegionTypeService _service;

        public GetAllRegionTypesQueryHandler(IRegionTypeService service)
        {
            _service = service;
        }

        public async Task<Result<PaginatedResult<RegionTypeResponse>>> Handle(GetAllRegionTypesQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
