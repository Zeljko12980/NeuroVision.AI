using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LegalSuccessor.Query.GetByKey
{
    public sealed record GetLegalSuccessorByKeyQuery(string SuccessorCountryCode, string PredecessorCountryCode) : IQuery<Result<LegalSuccessorResponse>>;
}
