using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Settlement.Query.GetByKey
{
    public sealed class GetSettlementByKeyQueryHandler : IQueryHandler<GetSettlementByKeyQuery, Result<SettlementResponse>>
    {
        private readonly ISettlementService _service;

        public GetSettlementByKeyQueryHandler(ISettlementService service)
        {
            _service = service;
        }

        public async Task<Result<SettlementResponse>> Handle(GetSettlementByKeyQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByKeyAsync(query.CountryCode, query.Code, cancellationToken);
        }
    }
}
