using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Settlement.Query.GetAll
{
    public sealed record GetAllSettlementsQuery(GetSettlementsRequest Request) : IQuery<Result<PaginatedResult<SettlementResponse>>>;
}
