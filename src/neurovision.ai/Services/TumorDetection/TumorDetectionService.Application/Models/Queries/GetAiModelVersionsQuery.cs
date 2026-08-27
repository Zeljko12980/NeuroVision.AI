using MediatR;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Application.Common.Responses;
using TumorDetectionService.Application.Models.Commands;

namespace TumorDetectionService.Application.Models.Queries;

public record GetAiModelVersionsQuery() : IRequest<IReadOnlyList<AiModelVersionResponse>>;

public class GetAiModelVersionsQueryHandler
    : IRequestHandler<GetAiModelVersionsQuery, IReadOnlyList<AiModelVersionResponse>>
{
    private readonly IAiModelVersionRepository _models;

    public GetAiModelVersionsQueryHandler(IAiModelVersionRepository models) => _models = models;

    public async Task<IReadOnlyList<AiModelVersionResponse>> Handle(
        GetAiModelVersionsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await _models.GetAllAsync(cancellationToken);
        return items.Select(RegisterAiModelVersionCommandHandler.Map).ToList();
    }
}
