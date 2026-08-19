using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.MunicipalitySettlementCoverage.Query.GetAll
{
    public sealed record GetAllMunicipalitySettlementCoveragesQuery(GetMunicipalitySettlementCoveragesRequest Request) : IQuery<Result<PaginatedResult<MunicipalitySettlementCoverageResponse>>>;
}
