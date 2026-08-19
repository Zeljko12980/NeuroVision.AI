using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocationService.Application.Feature.Country.Command.Delete
{
    public sealed record DeleteCountryCommand(string Code) : ICommand<Result<bool>>;
    

public sealed class DeleteCountryCommandValidator : AbstractValidator<DeleteCountryCommand>
{
    public DeleteCountryCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
    }
}
}
