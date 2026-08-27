using BuildingBlocks.Exceptions;
using MediatR;
using TumorDetectionService.Application.Common;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Application.Common.Mapping;
using TumorDetectionService.Application.Common.Responses;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Application.Analyses.Queries;

public record GetAnalysisByIdQuery(Guid AnalysisId, TumorActor Actor) : IRequest<AnalysisResponse>;

public record SearchAnalysesQuery(
    TumorActor Actor,
    Guid? PatientId,
    DateTime? From,
    DateTime? To,
    AnalysisStatus? Status,
    bool? Archived,
    int Page = 1,
    int PageSize = 20) : IRequest<PaginatedResponse<AnalysisResponse>>;

public record GetBrainScansQuery(TumorActor Actor, Guid? PatientId, int Page = 1, int PageSize = 20)
    : IRequest<PaginatedResponse<BrainScanResponse>>;

public record GetStatisticsQuery() : IRequest<AnalysisStatisticsResponse>;

public class GetAnalysisByIdQueryHandler : IRequestHandler<GetAnalysisByIdQuery, AnalysisResponse>
{
    private readonly ITumorAnalysisRepository _analyses;

    public GetAnalysisByIdQueryHandler(ITumorAnalysisRepository analyses) => _analyses = analyses;

    public async Task<AnalysisResponse> Handle(GetAnalysisByIdQuery request, CancellationToken cancellationToken)
    {
        var analysis = await _analyses.GetByIdWithDetailsAsync(request.AnalysisId, cancellationToken)
            ?? throw new NotFoundException($"Analysis {request.AnalysisId} not found.");
        TumorAccess.EnsureCanAccessAnalysis(request.Actor, analysis);
        return AnalysisMapper.ToResponse(analysis);
    }
}

public class SearchAnalysesQueryHandler : IRequestHandler<SearchAnalysesQuery, PaginatedResponse<AnalysisResponse>>
{
    private readonly ITumorAnalysisRepository _analyses;

    public SearchAnalysesQueryHandler(ITumorAnalysisRepository analyses) => _analyses = analyses;

    public async Task<PaginatedResponse<AnalysisResponse>> Handle(
        SearchAnalysesQuery request,
        CancellationToken cancellationToken)
    {
        var patientId = TumorAccess.ResolvePatientFilter(request.Actor, request.PatientId);
        DateTime? to = request.To;
        if (to.HasValue && to.Value.TimeOfDay == TimeSpan.Zero)
            to = to.Value.Date.AddDays(1).AddTicks(-1);

        var (items, total) = await _analyses.SearchAsync(
            patientId,
            request.From,
            to,
            request.Status,
            request.Archived,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PaginatedResponse<AnalysisResponse>(
            items.Select(AnalysisMapper.ToResponse).ToList(),
            total,
            request.Page,
            request.PageSize);
    }
}

public class GetBrainScansQueryHandler : IRequestHandler<GetBrainScansQuery, PaginatedResponse<BrainScanResponse>>
{
    private readonly IBrainScanRepository _scans;

    public GetBrainScansQueryHandler(IBrainScanRepository scans) => _scans = scans;

    public async Task<PaginatedResponse<BrainScanResponse>> Handle(
        GetBrainScansQuery request,
        CancellationToken cancellationToken)
    {
        var patientId = TumorAccess.ResolvePatientFilter(request.Actor, request.PatientId);
        var (items, total) = await _scans.GetByPatientAsync(
            patientId,
            request.Page,
            request.PageSize,
            cancellationToken);

        var mapped = items.Select(s => new BrainScanResponse(
            s.Id,
            s.PatientId,
            s.FileName,
            s.ScanType.ToString(),
            s.FileSizeBytes,
            s.UploadedAt,
            s.Analyses.Count)).ToList();

        return new PaginatedResponse<BrainScanResponse>(mapped, total, request.Page, request.PageSize);
    }
}

public class GetStatisticsQueryHandler : IRequestHandler<GetStatisticsQuery, AnalysisStatisticsResponse>
{
    private readonly ITumorAnalysisRepository _analyses;
    private readonly IBrainScanRepository _scans;

    public GetStatisticsQueryHandler(
        ITumorAnalysisRepository analyses,
        IBrainScanRepository scans)
    {
        _analyses = analyses;
        _scans = scans;
    }

    public async Task<AnalysisStatisticsResponse> Handle(
        GetStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        var completed = await _analyses.CountCompletedAsync(cancellationToken);
        var scanCount = await _scans.CountAsync(cancellationToken);
        return new AnalysisStatisticsResponse(completed, scanCount);
    }
}

public record GetAnalysisErrorLogsQuery(TumorActor Actor, int Page = 1, int PageSize = 20)
    : IRequest<PaginatedResponse<AnalysisErrorLogResponse>>;

public class GetAnalysisErrorLogsQueryHandler
    : IRequestHandler<GetAnalysisErrorLogsQuery, PaginatedResponse<AnalysisErrorLogResponse>>
{
    private readonly IAnalysisErrorLogRepository _errors;

    public GetAnalysisErrorLogsQueryHandler(IAnalysisErrorLogRepository errors) => _errors = errors;

    public async Task<PaginatedResponse<AnalysisErrorLogResponse>> Handle(
        GetAnalysisErrorLogsQuery request,
        CancellationToken cancellationToken)
    {
        TumorAccess.EnsureStaff(request.Actor);

        var (items, total) = await _errors.GetRecentAsync(request.Page, request.PageSize, cancellationToken);
        var mapped = items.Select(x => new AnalysisErrorLogResponse(
            x.Id,
            x.TumorAnalysisId,
            x.Message,
            x.Details,
            x.OccurredAt)).ToList();

        return new PaginatedResponse<AnalysisErrorLogResponse>(mapped, total, request.Page, request.PageSize);
    }
}
