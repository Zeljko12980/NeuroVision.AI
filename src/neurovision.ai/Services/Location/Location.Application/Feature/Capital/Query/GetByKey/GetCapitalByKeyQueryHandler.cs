using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Capital.Query.GetByKey
{
    public sealed class GetCapitalByKeyQueryHandler : IQueryHandler<GetCapitalByKeyQuery, Result<CapitalResponse>>
    {
        private readonly ICapitalService _service;

        public GetCapitalByKeyQueryHandler(ICapitalService service)
        {
            _service = service;
        }

        public async Task<Result<CapitalResponse>> Handle(GetCapitalByKeyQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByKeyAsync(query.CountryCode, query.SettlementCode, query.SequenceNumber, cancellationToken);
        }
    }
}
