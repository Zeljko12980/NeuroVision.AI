using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitution.Query.GetByKey
{
    public sealed class GetHealthInstitutionByKeyQueryHandler : IQueryHandler<GetHealthInstitutionByKeyQuery, Result<HealthInstitutionResponse>>
    {
        private readonly IHealthInstitutionService _service;

        public GetHealthInstitutionByKeyQueryHandler(IHealthInstitutionService service)
        {
            _service = service;
        }

        public async Task<Result<HealthInstitutionResponse>> Handle(GetHealthInstitutionByKeyQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByKeyAsync(query.Id, cancellationToken);
        }
    }
}
