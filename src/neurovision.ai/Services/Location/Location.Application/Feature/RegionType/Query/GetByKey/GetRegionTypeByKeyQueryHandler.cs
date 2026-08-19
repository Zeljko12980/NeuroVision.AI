using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionType.Query.GetByKey
{
    public sealed class GetRegionTypeByKeyQueryHandler : IQueryHandler<GetRegionTypeByKeyQuery, Result<RegionTypeResponse>>
    {
        private readonly IRegionTypeService _service;

        public GetRegionTypeByKeyQueryHandler(IRegionTypeService service)
        {
            _service = service;
        }

        public async Task<Result<RegionTypeResponse>> Handle(GetRegionTypeByKeyQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByKeyAsync(query.Code, cancellationToken);
        }
    }
}
