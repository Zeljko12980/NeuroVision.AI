using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Feature.CountryComposition.Command.Update
{
    public sealed class UpdateCountryCompositionCommandHandler : ICommandHandler<UpdateCountryCompositionCommand, Result<CountryCompositionResponse>>
    {
        private readonly ICountryCompositionService _service;

        public UpdateCountryCompositionCommandHandler(ICountryCompositionService service)
        {
            _service = service;
        }

        public async Task<Result<CountryCompositionResponse>> Handle(UpdateCountryCompositionCommand command, CancellationToken cancellationToken)
        {
            return await _service.UpdateAsync(command.UnionCountryCode, command.MemberCountryCode, command.SequenceNumber, command.Request, cancellationToken);
        }
    }
}
