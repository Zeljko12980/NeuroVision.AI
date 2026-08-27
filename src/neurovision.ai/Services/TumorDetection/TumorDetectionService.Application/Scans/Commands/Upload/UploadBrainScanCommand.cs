using BuildingBlocks.Persistence;
using MediatR;
using TumorDetectionService.Application.Common;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Application.Common.Responses;
using TumorDetectionService.Domain.Entities;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Application.Scans.Commands.Upload;

public record UploadBrainScanCommand(
    Guid PatientId,
    TumorActor Actor,
    string FileName,
    string ContentType,
    ScanType ScanType,
    long FileSizeBytes,
    Stream Content) : IRequest<BrainScanResponse>;

public class UploadBrainScanCommandHandler : IRequestHandler<UploadBrainScanCommand, BrainScanResponse>
{
    private readonly IBrainScanRepository _scans;
    private readonly IScanStorageService _storage;
    private readonly IUnitOfWork _unitOfWork;

    public UploadBrainScanCommandHandler(
        IBrainScanRepository scans,
        IScanStorageService storage,
        IUnitOfWork unitOfWork)
    {
        _scans = scans;
        _storage = storage;
        _unitOfWork = unitOfWork;
    }

    public async Task<BrainScanResponse> Handle(UploadBrainScanCommand request, CancellationToken cancellationToken)
    {
        TumorAccess.EnsureCanUploadFor(request.Actor, request.PatientId);

        var scanId = Guid.NewGuid();
        var path = await _storage.SaveScanAsync(request.Content, request.FileName, scanId, cancellationToken);

        var scan = BrainScan.Create(
            request.PatientId,
            request.Actor.UserId,
            request.FileName,
            path,
            request.ContentType,
            request.ScanType,
            request.FileSizeBytes,
            scanId);

        await _scans.AddAsync(scan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BrainScanResponse(
            scan.Id,
            scan.PatientId,
            scan.FileName,
            scan.ScanType.ToString(),
            scan.FileSizeBytes,
            scan.UploadedAt,
            0);
    }
}
