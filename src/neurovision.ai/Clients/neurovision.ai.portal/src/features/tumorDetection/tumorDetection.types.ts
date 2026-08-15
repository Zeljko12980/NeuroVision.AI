export type ScanType = "Mri" | "Ct";

export type AnalysisStatus =
    | "Pending"
    | "Processing"
    | "Completed"
    | "Failed"
    | "Corrected";

export interface BrainScanResponse {
    id: string;
    patientId: string;
    fileName: string;
    scanType: ScanType;
    fileSizeBytes: number;
    uploadedAt: string;
    analysisCount: number;
}

export interface DetectionFindingResponse {
    className: string;
    confidence: number;
    xCenter: number;
    yCenter: number;
    width: number;
    height: number;
}

export interface AnalysisResponse {
    id: string;
    brainScanId: string;
    patientId: string;
    scanFileName: string;
    status: AnalysisStatus;
    requestedAt: string;
    completedAt?: string | null;
    overallConfidence?: number | null;
    classificationClass?: string | null;
    classificationConfidence?: number | null;
    tumorAreaRatio?: number | null;
    detections: DetectionFindingResponse[];
    reportFilePath?: string | null;
    hasAnnotatedImage: boolean;
    hasDetectionImage: boolean;
    hasSegmentationImage: boolean;
    hasMaskImage: boolean;
    hasPdfReport: boolean;
    pdfGeneratedAt?: string | null;
}

export interface AnalysisReportResponse {
    analysisId: string;
    brainScanId: string;
    patientId: string;
    scanFileName: string;
    status: AnalysisStatus;
    completedAt?: string | null;
    pdfGeneratedAt?: string | null;
    classificationClass?: string | null;
    overallConfidence?: number | null;
}

export interface PaginatedResponse<T> {
    items: T[];
    total: number;
    page: number;
    pageSize: number;
}

export interface AnalysisStatisticsResponse {
    totalCompletedAnalyses: number;
    totalScans: number;
}

export interface AiModelVersionResponse {
    id: string;
    taskType: string;
    versionLabel: string;
    runId: string;
    isActive: boolean;
    registeredAt: string;
}

export interface CommentResponse {
    id: string;
    tumorAnalysisId: string;
    authorUserId: string;
    content: string;
    createdAt: string;
    updatedAt?: string | null;
}

export interface AnalysisStatusNotification {
    analysisId: string;
    brainScanId: string;
    patientId: string;
    status: AnalysisStatus;
}
