using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.RegionComposition.Command.Delete
{
    public sealed class DeleteRegionCompositionCommandHandler : ICommandHandler<DeleteRegionCompositionCommand, Result<bool>>
    {
        private readonly IRegionCompositionService _service;

        public DeleteRegionCompositionCommandHandler(IRegionCompositionService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteRegionCompositionCommand command, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(command.ParentRegionTypeCode, command.ParentRegionCode, command.MemberRegionTypeCode, command.MemberRegionCode, cancellationToken);
        }
    }
}
