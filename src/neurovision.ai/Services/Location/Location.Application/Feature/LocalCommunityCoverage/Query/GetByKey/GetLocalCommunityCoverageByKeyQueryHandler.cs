using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LocalCommunityCoverage.Query.GetByKey
{
    public sealed class GetLocalCommunityCoverageByKeyQueryHandler : IQueryHandler<GetLocalCommunityCoverageByKeyQuery, Result<LocalCommunityCoverageResponse>>
    {
        private readonly ILocalCommunityCoverageService _service;

        public GetLocalCommunityCoverageByKeyQueryHandler(ILocalCommunityCoverageService service)
        {
            _service = service;
        }

        public async Task<Result<LocalCommunityCoverageResponse>> Handle(GetLocalCommunityCoverageByKeyQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByKeyAsync(query.CountryCode, query.MunicipalityCode, query.LocalCommunityIdentifier, query.SettlementCode, cancellationToken);
        }
    }
}
