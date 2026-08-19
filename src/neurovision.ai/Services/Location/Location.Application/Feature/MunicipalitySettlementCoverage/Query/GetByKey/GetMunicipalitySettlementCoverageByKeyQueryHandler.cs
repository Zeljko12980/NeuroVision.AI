using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.MunicipalitySettlementCoverage.Query.GetByKey
{
    public sealed class GetMunicipalitySettlementCoverageByKeyQueryHandler : IQueryHandler<GetMunicipalitySettlementCoverageByKeyQuery, Result<MunicipalitySettlementCoverageResponse>>
    {
        private readonly IMunicipalitySettlementCoverageService _service;

        public GetMunicipalitySettlementCoverageByKeyQueryHandler(IMunicipalitySettlementCoverageService service)
        {
            _service = service;
        }

        public async Task<Result<MunicipalitySettlementCoverageResponse>> Handle(GetMunicipalitySettlementCoverageByKeyQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByKeyAsync(query.CountryCode, query.MunicipalityCode, query.SettlementCode, cancellationToken);
        }
    }
}
