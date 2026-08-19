using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Municipality.Query.GetByKey
{
    public sealed class GetMunicipalityByKeyQueryHandler : IQueryHandler<GetMunicipalityByKeyQuery, Result<MunicipalityResponse>>
    {
        private readonly IMunicipalityService _service;

        public GetMunicipalityByKeyQueryHandler(IMunicipalityService service)
        {
            _service = service;
        }

        public async Task<Result<MunicipalityResponse>> Handle(GetMunicipalityByKeyQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByKeyAsync(query.CountryCode, query.Code, cancellationToken);
        }
    }
}
