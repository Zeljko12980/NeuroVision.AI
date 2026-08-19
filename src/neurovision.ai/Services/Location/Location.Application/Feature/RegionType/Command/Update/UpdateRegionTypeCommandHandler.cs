using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionType.Command.Update
{
    public sealed class UpdateRegionTypeCommandHandler : ICommandHandler<UpdateRegionTypeCommand, Result<RegionTypeResponse>>
    {
        private readonly IRegionTypeService _service;

        public UpdateRegionTypeCommandHandler(IRegionTypeService service)
        {
            _service = service;
        }

        public async Task<Result<RegionTypeResponse>> Handle(UpdateRegionTypeCommand command, CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(command.Code, command.Request, cancellationToken);
        }
    }
}
