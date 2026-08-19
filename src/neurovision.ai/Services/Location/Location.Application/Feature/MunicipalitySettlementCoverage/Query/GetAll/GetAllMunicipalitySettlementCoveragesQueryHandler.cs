using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.MunicipalitySettlementCoverage.Query.GetAll
{
    public sealed class GetAllMunicipalitySettlementCoveragesQueryHandler : IQueryHandler<GetAllMunicipalitySettlementCoveragesQuery, Result<PaginatedResult<MunicipalitySettlementCoverageResponse>>>
    {
        private readonly IMunicipalitySettlementCoverageService _service;

        public GetAllMunicipalitySettlementCoveragesQueryHandler(IMunicipalitySettlementCoverageService service)
        {
            _service = service;
        }

        public async Task<Result<PaginatedResult<MunicipalitySettlementCoverageResponse>>> Handle(GetAllMunicipalitySettlementCoveragesQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync(query.Request, cancellationToken);
        }
    }
}
