using BuildingBlocks.CQRS;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;

namespace LocationService.Application.Feature.CountryComposition.Command.Delete
{
    public sealed class DeleteCountryCompositionCommandHandler : ICommandHandler<DeleteCountryCompositionCommand, Result<bool>>
    {
        private readonly ICountryCompositionService _service;

        public DeleteCountryCompositionCommandHandler(ICountryCompositionService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteCountryCompositionCommand command, CancellationToken cancellationToken)
        {
            return await _service.DeleteAsync(command.UnionCountryCode, command.MemberCountryCode, command.SequenceNumber, cancellationToken);
        }
    }
}
