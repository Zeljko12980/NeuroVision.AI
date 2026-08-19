using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.GovernmentType.Command.Create
{
    public sealed record CreateGovernmentTypeCommand(CreateGovernmentTypeRequest Request) : ICommand<Result<GovernmentTypeResponse>>;

public sealed class CreateGovernmentTypeCommandValidator : AbstractValidator<CreateGovernmentTypeCommand>
{
    public CreateGovernmentTypeCommandValidator()
    {
        RuleFor(x => x.Request.Code).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(120);
    }
}
}
