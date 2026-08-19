using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace LocationService.Application.Feature.HealthInstitutionType.Command.Delete
{
    public sealed record DeleteHealthInstitutionTypeCommand(string Code) : ICommand<Result<bool>>;
}
