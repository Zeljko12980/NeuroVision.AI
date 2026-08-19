using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.CountryComposition.Command.Create
{
    public sealed record CreateCountryCompositionCommand(CreateCountryCompositionRequest Request) : ICommand<Result<CountryCompositionResponse>>;

public sealed class CreateCountryCompositionCommandValidator : AbstractValidator<CreateCountryCompositionCommand>
{
    public CreateCountryCompositionCommandValidator()
    {
        RuleFor(x => x.Request.UnionCountryCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.Request.MemberCountryCode).NotEmpty().MaximumLength(3);
    }
}
}
