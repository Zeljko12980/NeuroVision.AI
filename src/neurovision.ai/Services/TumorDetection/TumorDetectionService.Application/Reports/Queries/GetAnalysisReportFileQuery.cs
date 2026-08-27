using BuildingBlocks.Exceptions;
using MediatR;
using TumorDetectionService.Application.Common;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Application.Common.Responses;

namespace TumorDetectionService.Application.Reports.Queries;

public record GetAnalysisReportFileQuery(Guid AnalysisId, TumorActor Actor)
    : IRequest<AnalysisFileResult>;

public class GetAnalysisReportFileQueryHandler
    : IRequestHandler<GetAnalysisReportFileQuery, AnalysisFileResult>
{
    private readonly ITumorAnalysisRepository _analyses;
    private readonly IReportStorageService _reportStorage;

    public GetAnalysisReportFileQueryHandler(
        ITumorAnalysisRepository analyses,
        IReportStorageService reportStorage)
    {
        _analyses = analyses;
        _reportStorage = reportStorage;
    }

    public async Task<AnalysisFileResult> Handle(
        GetAnalysisReportFileQuery request,
        CancellationToken cancellationToken)
    {
        var analysis = await _analyses.GetByIdWithDetailsAsync(request.AnalysisId, cancellationToken)
            ?? throw new NotFoundException($"Analysis {request.AnalysisId} not found.");

        TumorAccess.EnsureCanAccessAnalysis(request.Actor, analysis);

        var path = analysis.PdfReportPath ?? _reportStorage.GetReportPath(analysis.Id);
        if (path is null || !File.Exists(path))
            throw new NotFoundException($"PDF report for analysis {request.AnalysisId} was not found.");

        return new AnalysisFileResult(path, $"analysis-report-{analysis.Id:N}.pdf");
    }
}
