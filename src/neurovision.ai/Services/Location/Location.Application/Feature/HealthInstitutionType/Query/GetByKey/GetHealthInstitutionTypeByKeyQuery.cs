using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitutionType.Query.GetByKey
{
    public sealed record GetHealthInstitutionTypeByKeyQuery(string Code) : IQuery<Result<HealthInstitutionTypeResponse>>;
}
