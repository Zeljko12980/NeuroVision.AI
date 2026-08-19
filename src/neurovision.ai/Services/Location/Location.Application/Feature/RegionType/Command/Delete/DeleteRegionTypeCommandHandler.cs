using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.RegionType.Command.Delete
{
    public sealed class DeleteRegionTypeCommandHandler : ICommandHandler<DeleteRegionTypeCommand, Result<bool>>
    {
        private readonly IRegionTypeService _service;

        public DeleteRegionTypeCommandHandler(IRegionTypeService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteRegionTypeCommand command, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(command.Code, cancellationToken);
        }
    }
}
