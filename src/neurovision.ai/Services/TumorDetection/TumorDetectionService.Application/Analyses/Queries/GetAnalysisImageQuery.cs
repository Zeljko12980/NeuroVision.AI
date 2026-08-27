using BuildingBlocks.Exceptions;
using MediatR;
using TumorDetectionService.Application.Common;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Application.Common.Mapping;
using TumorDetectionService.Application.Common.Responses;

namespace TumorDetectionService.Application.Analyses.Queries;

public record GetAnalysisImageQuery(Guid AnalysisId, string Kind, TumorActor Actor)
    : IRequest<AnalysisFileResult>;

public class GetAnalysisImageQueryHandler : IRequestHandler<GetAnalysisImageQuery, AnalysisFileResult>
{
    private readonly ITumorAnalysisRepository _analyses;

    public GetAnalysisImageQueryHandler(ITumorAnalysisRepository analyses) => _analyses = analyses;

    public async Task<AnalysisFileResult> Handle(
        GetAnalysisImageQuery request,
        CancellationToken cancellationToken)
    {
        var analysis = await _analyses.GetByIdWithDetailsAsync(request.AnalysisId, cancellationToken)
            ?? throw new NotFoundException($"Analysis {request.AnalysisId} not found.");

        TumorAccess.EnsureCanAccessAnalysis(request.Actor, analysis);

        var path = AnalysisImagePaths.ResolveFilePath(analysis, request.Kind)
            ?? throw new NotFoundException($"Image '{request.Kind}' was not found for analysis {request.AnalysisId}.");

        return new AnalysisFileResult(path, null);
    }
}
