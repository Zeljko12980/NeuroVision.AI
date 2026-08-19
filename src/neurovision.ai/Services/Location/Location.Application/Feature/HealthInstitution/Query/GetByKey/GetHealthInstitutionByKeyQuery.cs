using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitution.Query.GetByKey
{
    public sealed record GetHealthInstitutionByKeyQuery(int Id) : IQuery<Result<HealthInstitutionResponse>>;
}
