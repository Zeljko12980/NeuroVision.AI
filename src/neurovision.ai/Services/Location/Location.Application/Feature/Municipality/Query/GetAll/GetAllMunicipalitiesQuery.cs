using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Municipality.Query.GetAll
{
    public sealed record GetAllMunicipalitiesQuery(GetMunicipalitiesRequest Request) : IQuery<Result<PaginatedResult<MunicipalityResponse>>>;
}
