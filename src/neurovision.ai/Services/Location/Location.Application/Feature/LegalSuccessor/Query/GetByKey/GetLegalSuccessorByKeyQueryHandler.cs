using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LegalSuccessor.Query.GetByKey
{
    public sealed class GetLegalSuccessorByKeyQueryHandler : IQueryHandler<GetLegalSuccessorByKeyQuery, Result<LegalSuccessorResponse>>
    {
        private readonly ILegalSuccessorService _service;

        public GetLegalSuccessorByKeyQueryHandler(ILegalSuccessorService service)
        {
            _service = service;
        }

        public async Task<Result<LegalSuccessorResponse>> Handle(GetLegalSuccessorByKeyQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByKeyAsync(query.SuccessorCountryCode, query.PredecessorCountryCode, cancellationToken);
        }
    }
}
