using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.HealthInstitution.Command.Delete
{
    public sealed record DeleteHealthInstitutionCommand(int Id) : ICommand<Result<bool>>;
}
