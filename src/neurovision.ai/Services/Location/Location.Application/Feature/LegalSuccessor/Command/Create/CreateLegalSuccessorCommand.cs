using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.LegalSuccessor.Command.Create
{
    public sealed record CreateLegalSuccessorCommand(CreateLegalSuccessorRequest Request) : ICommand<Result<LegalSuccessorResponse>>;
}
