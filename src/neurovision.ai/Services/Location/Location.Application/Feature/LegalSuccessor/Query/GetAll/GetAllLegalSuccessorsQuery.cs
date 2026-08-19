using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LegalSuccessor.Query.GetAll
{
    public sealed record GetAllLegalSuccessorsQuery(GetLegalSuccessorsRequest Request) : IQuery<Result<PaginatedResult<LegalSuccessorResponse>>>;
}
