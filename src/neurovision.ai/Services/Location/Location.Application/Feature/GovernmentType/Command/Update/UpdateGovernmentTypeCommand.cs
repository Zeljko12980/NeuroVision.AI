using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.GovernmentType.Command.Update
{
    public sealed record UpdateGovernmentTypeCommand(UpdateGovernmentTypeRequest Request, string Code) : ICommand<Result<GovernmentTypeResponse>>;

public sealed class UpdateGovernmentTypeCommandValidator : AbstractValidator<UpdateGovernmentTypeCommand>
{
    public UpdateGovernmentTypeCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(120);
    }
}
}
