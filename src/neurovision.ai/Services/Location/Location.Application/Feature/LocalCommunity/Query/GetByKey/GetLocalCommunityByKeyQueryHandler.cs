using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LocalCommunity.Query.GetByKey
{
    public sealed class GetLocalCommunityByKeyQueryHandler : IQueryHandler<GetLocalCommunityByKeyQuery, Result<LocalCommunityResponse>>
    {
        private readonly ILocalCommunityService _service;

        public GetLocalCommunityByKeyQueryHandler(ILocalCommunityService service)
        {
            _service = service;
        }

        public async Task<Result<LocalCommunityResponse>> Handle(GetLocalCommunityByKeyQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByKeyAsync(query.CountryCode, query.MunicipalityCode, query.Identifier, cancellationToken);
        }
    }
}
