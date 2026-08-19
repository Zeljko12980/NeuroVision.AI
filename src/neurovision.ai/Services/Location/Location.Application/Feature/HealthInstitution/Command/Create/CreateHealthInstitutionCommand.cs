using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.HealthInstitution.Command.Create
{
    public sealed record CreateHealthInstitutionCommand(CreateHealthInstitutionRequest Request) : ICommand<Result<HealthInstitutionResponse>>;
}
