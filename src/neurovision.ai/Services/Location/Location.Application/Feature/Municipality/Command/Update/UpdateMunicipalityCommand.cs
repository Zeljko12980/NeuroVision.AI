using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Municipality.Command.Update
{
    public sealed record UpdateMunicipalityCommand(UpdateMunicipalityRequest Request, string CountryCode, int Code) : ICommand<Result<MunicipalityResponse>>;

public sealed class UpdateMunicipalityCommandValidator : AbstractValidator<UpdateMunicipalityCommand>
{
    public UpdateMunicipalityCommandValidator()
    {
        RuleFor(x => x.CountryCode).NotEmpty();
        RuleFor(x => x.Request.Name).NotEmpty().MaximumLength(120);
    }
}
}
