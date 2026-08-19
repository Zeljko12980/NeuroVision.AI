using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Region.Command.Update
{
    public sealed class UpdateRegionCommandHandler : ICommandHandler<UpdateRegionCommand, Result<RegionResponse>>
    {
        private readonly IRegionService _service;

        public UpdateRegionCommandHandler(IRegionService service)
        {
            _service = service;
        }

        public async Task<Result<RegionResponse>> Handle(UpdateRegionCommand command, CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(command.TypeCode, command.Code, command.Request, cancellationToken);
        }
    }
}
