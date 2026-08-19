using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.CountryComposition.Query.GetByKey
{
    public sealed class GetCountryCompositionByKeyQueryHandler : IQueryHandler<GetCountryCompositionByKeyQuery, Result<CountryCompositionResponse>>
    {
        private readonly ICountryCompositionService _service;

        public GetCountryCompositionByKeyQueryHandler(ICountryCompositionService service)
        {
            _service = service;
        }

        public async Task<Result<CountryCompositionResponse>> Handle(GetCountryCompositionByKeyQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByKeyAsync(query.UnionCountryCode, query.MemberCountryCode, query.SequenceNumber, cancellationToken);
        }
    }
}
