using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Region.Query.GetByKey
{
    public sealed class GetRegionByKeyQueryHandler : IQueryHandler<GetRegionByKeyQuery, Result<RegionResponse>>
    {
        private readonly IRegionService _service;

        public GetRegionByKeyQueryHandler(IRegionService service)
        {
            _service = service;
        }

        public async Task<Result<RegionResponse>> Handle(GetRegionByKeyQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByKeyAsync(query.TypeCode, query.Code, cancellationToken);
        }
    }
}
