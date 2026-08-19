import { del, get, post, put } from "../../api/api";
import type {
    AiModelVersionResponse,
    AnalysisReportResponse,
    AnalysisResponse,
    AnalysisStatisticsResponse,
    BrainScanResponse,
    CommentResponse,
    PaginatedResponse,
    ScanType,
} from "./tumorDetection.types";

const base = "/tumor";

export const fetchScans = async (
    patientId?: string,
    page = 1,
    pageSize = 20
): Promise<PaginatedResponse<BrainScanResponse>> =>
    get(`${base}/scans?${new URLSearchParams({
        ...(patientId ? { patientId } : {}),
        page: String(page),
        pageSize: String(pageSize),
    }).toString()}`);

export const uploadScan = async (params: {
    patientId: string;
    uploadedByUserId: string;
    scanType: ScanType;
    file: File;
}): Promise<BrainScanResponse> => {
    const formData = new FormData();
    formData.append("patientId", params.patientId);
    formData.append("uploadedByUserId", params.uploadedByUserId);
    formData.append("scanType", params.scanType);
    formData.append("file", params.file);
    return post(`${base}/scans`, formData);
};

export const startAnalysis = async (
    brainScanId: string,
    requestedByUserId: string
): Promise<AnalysisResponse> =>
    post(`${base}/analyses`, { brainScanId, requestedByUserId });

export const fetchAnalysis = async (analysisId: string): Promise<AnalysisResponse> =>
    get(`${base}/analyses/${analysisId}`);

export const searchAnalyses = async (params: {
    patientId?: string;
    from?: string;
    to?: string;
    status?: string;
    archived?: boolean;
    page?: number;
    pageSize?: number;
}): Promise<PaginatedResponse<AnalysisResponse>> => {
    const query = new URLSearchParams();
    if (params.patientId) query.set("patientId", params.patientId);
    if (params.from) query.set("from", params.from);
    if (params.to) query.set("to", params.to);
    if (params.status) query.set("status", params.status);
    if (params.archived !== undefined) query.set("archived", String(params.archived));
    query.set("page", String(params.page ?? 1));
    query.set("pageSize", String(params.pageSize ?? 20));
    return get(`${base}/analyses?${query.toString()}`);
};

export const fetchStatistics = async (): Promise<AnalysisStatisticsResponse> =>
    get(`${base}/analyses/statistics`);

export const fetchModelVersions = async (): Promise<AiModelVersionResponse[]> =>
    get(`${base}/models`);

export const registerModelVersion = async (payload: {
    taskType: number;
    versionLabel: string;
    runId: string;
    weightsPath: string;
    registeredByUserId: string;
    setActive?: boolean;
}): Promise<AiModelVersionResponse> => post(`${base}/models`, payload);

export const fetchComments = async (analysisId: string): Promise<CommentResponse[]> =>
    get(`${base}/analyses/${analysisId}/comments`);

export const addComment = async (
    analysisId: string,
    authorUserId: string,
    content: string
): Promise<CommentResponse> =>
    post(`${base}/analyses/${analysisId}/comments`, { authorUserId, content });

export const updateComment = async (
    analysisId: string,
    commentId: string,
    authorUserId: string,
    content: string
): Promise<CommentResponse> =>
    put(`${base}/analyses/${analysisId}/comments/${commentId}`, { authorUserId, content });

export const deleteComment = async (
    analysisId: string,
    commentId: string,
    authorUserId: string
): Promise<void> =>
    del(`${base}/analyses/${analysisId}/comments/${commentId}?authorUserId=${authorUserId}`);

export const applyManualCorrection = async (
    analysisId: string,
    payload: {
        correctedByUserId: string;
        correctedClass: number;
        notes?: string;
    }
): Promise<AnalysisResponse> =>
    post(`${base}/analyses/${analysisId}/correction`, payload);

export const generateAnalysisReport = async (
    analysisId: string,
    payload?: { doctorName?: string; certificateId?: string; userId?: string }
): Promise<AnalysisResponse> =>
    post(`${base}/analyses/${analysisId}/report`, payload ?? {});

export const searchAnalysisReports = async (params: {
    patientId?: string;
    page?: number;
    pageSize?: number;
}): Promise<PaginatedResponse<AnalysisReportResponse>> => {
    const query = new URLSearchParams();
    if (params.patientId) query.set("patientId", params.patientId);
    query.set("page", String(params.page ?? 1));
    query.set("pageSize", String(params.pageSize ?? 20));
    return get(`${base}/reports?${query.toString()}`);
};

export const downloadAnalysisReport = async (analysisId: string): Promise<void> => {
    const token = localStorage.getItem("token");
    const response = await fetch(
        `${import.meta.env.VITE_API_URL}/tumor/analyses/${analysisId}/report`,
        {
            method: "GET",
            headers: {
                ...(token && { Authorization: `Bearer ${token}` }),
            },
        }
    );

    if (!response.ok) {
        throw new Error("Failed to download report");
    }

    const blob = await response.blob();
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = `analysis-report-${analysisId}.pdf`;
    document.body.appendChild(link);
    link.click();
    link.remove();
    URL.revokeObjectURL(url);
};
