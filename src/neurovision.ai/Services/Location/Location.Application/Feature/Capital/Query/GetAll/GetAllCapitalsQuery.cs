using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Capital.Query.GetAll
{
    public sealed record GetAllCapitalsQuery(GetCapitalsRequest Request) : IQuery<Result<PaginatedResult<CapitalResponse>>>;
}
