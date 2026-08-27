using MediatR;
using TumorDetectionService.Application.Common;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Application.Common.Responses;

namespace TumorDetectionService.Application.Reports.Queries;

public record SearchAnalysisReportsQuery(
    TumorActor Actor,
    Guid? PatientId,
    int Page = 1,
    int PageSize = 20) : IRequest<PaginatedResponse<AnalysisReportResponse>>;

public class SearchAnalysisReportsQueryHandler
    : IRequestHandler<SearchAnalysisReportsQuery, PaginatedResponse<AnalysisReportResponse>>
{
    private readonly ITumorAnalysisRepository _analyses;

    public SearchAnalysisReportsQueryHandler(ITumorAnalysisRepository analyses) => _analyses = analyses;

    public async Task<PaginatedResponse<AnalysisReportResponse>> Handle(
        SearchAnalysisReportsQuery request,
        CancellationToken cancellationToken)
    {
        var patientId = TumorAccess.ResolvePatientFilter(request.Actor, request.PatientId);
        var (items, total) = await _analyses.SearchReportsAsync(
            patientId,
            request.Page,
            request.PageSize,
            cancellationToken);

        var mapped = items.Select(a => new AnalysisReportResponse(
            a.Id,
            a.BrainScanId,
            a.BrainScan.PatientId,
            a.BrainScan.FileName,
            a.Status.ToString(),
            a.CompletedAt,
            a.PdfGeneratedAt,
            a.Classification?.PredictedClass.ToString(),
            a.OverallConfidence)).ToList();

        return new PaginatedResponse<AnalysisReportResponse>(
            mapped,
            total,
            request.Page,
            request.PageSize);
    }
}
