using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.GovernmentType.Command.Delete
{
    public sealed class DeleteGovernmentTypeCommandHandler : ICommandHandler<DeleteGovernmentTypeCommand, Result<bool>>
    {
        private readonly IGovernmentTypeService _service;

        public DeleteGovernmentTypeCommandHandler(IGovernmentTypeService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteGovernmentTypeCommand command, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(command.Code, cancellationToken);
        }
    }
}
