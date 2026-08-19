using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.Region.Command.Delete
{
    public sealed class DeleteRegionCommandHandler : ICommandHandler<DeleteRegionCommand, Result<bool>>
    {
        private readonly IRegionService _service;

        public DeleteRegionCommandHandler(IRegionService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteRegionCommand command, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(command.TypeCode, command.Code, cancellationToken);
        }
    }
}
