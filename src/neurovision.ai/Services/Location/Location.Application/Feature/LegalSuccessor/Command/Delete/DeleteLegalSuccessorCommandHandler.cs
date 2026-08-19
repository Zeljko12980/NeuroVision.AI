using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.LegalSuccessor.Command.Delete
{
    public sealed class DeleteLegalSuccessorCommandHandler : ICommandHandler<DeleteLegalSuccessorCommand, Result<bool>>
    {
        private readonly ILegalSuccessorService _service;

        public DeleteLegalSuccessorCommandHandler(ILegalSuccessorService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteLegalSuccessorCommand command, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(command.SuccessorCountryCode, command.PredecessorCountryCode, cancellationToken);
        }
    }
}
