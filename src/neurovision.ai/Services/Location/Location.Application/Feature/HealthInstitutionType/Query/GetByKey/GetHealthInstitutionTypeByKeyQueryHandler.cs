using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitutionType.Query.GetByKey
{
    public sealed class GetHealthInstitutionTypeByKeyQueryHandler : IQueryHandler<GetHealthInstitutionTypeByKeyQuery, Result<HealthInstitutionTypeResponse>>
    {
        private readonly IHealthInstitutionTypeService _service;

        public GetHealthInstitutionTypeByKeyQueryHandler(IHealthInstitutionTypeService service)
        {
            _service = service;
        }

        public async Task<Result<HealthInstitutionTypeResponse>> Handle(GetHealthInstitutionTypeByKeyQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByKeyAsync(query.Code, cancellationToken);
        }
    }
}
