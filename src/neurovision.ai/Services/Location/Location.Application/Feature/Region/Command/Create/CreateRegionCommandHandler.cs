using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.Region.Command.Create
{
    public sealed class CreateRegionCommandHandler : ICommandHandler<CreateRegionCommand, Result<RegionResponse>>
    {
        private readonly IRegionService _service;

        public CreateRegionCommandHandler(IRegionService service)
        {
            _service = service;
        }

        public async Task<Result<RegionResponse>> Handle(CreateRegionCommand command, CancellationToken cancellationToken)
        {
            return await _service.AddAsync(command.Request, cancellationToken);
        }
    }
}
