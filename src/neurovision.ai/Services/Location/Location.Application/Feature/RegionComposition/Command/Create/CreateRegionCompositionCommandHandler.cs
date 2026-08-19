using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.RegionComposition.Command.Create
{
    public sealed class CreateRegionCompositionCommandHandler : ICommandHandler<CreateRegionCompositionCommand, Result<RegionCompositionResponse>>
    {
        private readonly IRegionCompositionService _service;

        public CreateRegionCompositionCommandHandler(IRegionCompositionService service)
        {
            _service = service;
        }

        public async Task<Result<RegionCompositionResponse>> Handle(CreateRegionCompositionCommand command, CancellationToken cancellationToken)
        {
            return await _service.AddAsync(command.Request, cancellationToken);
        }
    }
}
