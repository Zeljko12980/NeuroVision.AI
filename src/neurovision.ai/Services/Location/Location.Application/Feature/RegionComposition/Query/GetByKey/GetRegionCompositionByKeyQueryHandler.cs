using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionComposition.Query.GetByKey
{
    public sealed class GetRegionCompositionByKeyQueryHandler : IQueryHandler<GetRegionCompositionByKeyQuery, Result<RegionCompositionResponse>>
    {
        private readonly IRegionCompositionService _service;

        public GetRegionCompositionByKeyQueryHandler(IRegionCompositionService service)
        {
            _service = service;
        }

        public async Task<Result<RegionCompositionResponse>> Handle(GetRegionCompositionByKeyQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByKeyAsync(query.ParentRegionTypeCode, query.ParentRegionCode, query.MemberRegionTypeCode, query.MemberRegionCode, cancellationToken);
        }
    }
}
