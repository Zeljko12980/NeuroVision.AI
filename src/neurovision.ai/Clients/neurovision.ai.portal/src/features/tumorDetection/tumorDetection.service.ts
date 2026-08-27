import { del, get, post, put } from "../../api/api";
import type {
    AiModelTypeResponse,
    AiModelVersionResponse,
    AnalysisErrorLogResponse,
    AnalysisReportResponse,
    AnalysisResponse,
    AnalysisStatisticsResponse,
    BrainScanResponse,
    ClinicalCatalogsResponse,
    ClinicalFollowUpResponse,
    CommentResponse,
    PaginatedResponse,
    ScanType,
    UpsertClinicalFollowUpRequest,
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
    scanType: ScanType;
    file: File;
}): Promise<BrainScanResponse> => {
    const formData = new FormData();
    formData.append("patientId", params.patientId);
    formData.append("scanType", params.scanType);
    formData.append("file", params.file);
    return post(`${base}/scans`, formData);
};

export const startAnalysis = async (
    brainScanId: string
): Promise<AnalysisResponse> =>
    post(`${base}/analyses`, { brainScanId });

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

export const fetchModelTypes = async (): Promise<AiModelTypeResponse[]> => {
    const result = await get(`${base}/model-types?pageIndex=0&pageSize=100`);
    return result?.data ?? [];
};

export const registerModelVersion = async (payload: {
    taskType: number;
    versionLabel: string;
    runId: string;
    weightsPath: string;
    setActive?: boolean;
}): Promise<AiModelVersionResponse> => post(`${base}/models`, payload);

export const activateModelVersion = async (id: string): Promise<AiModelVersionResponse> =>
    post(`${base}/models/${id}/activate`, {});

export const uploadModelVersion = async (payload: {
    taskType: number | string;
    versionLabel: string;
    runId?: string;
    setActive?: boolean;
    file: File;
}): Promise<AiModelVersionResponse> => {
    const formData = new FormData();
    formData.append("taskType", String(payload.taskType));
    formData.append("versionLabel", payload.versionLabel);
    if (payload.runId) formData.append("runId", payload.runId);
    formData.append("setActive", String(payload.setActive ?? true));
    formData.append("file", payload.file, payload.file.name);
    return post(`${base}/models/upload`, formData);
};

export const fetchAnalysisErrors = async (
    page = 1,
    pageSize = 10
): Promise<PaginatedResponse<AnalysisErrorLogResponse>> =>
    get(`${base}/analyses/errors?${new URLSearchParams({
        page: String(page),
        pageSize: String(pageSize),
    }).toString()}`);

export const fetchComments = async (analysisId: string): Promise<CommentResponse[]> =>
    get(`${base}/analyses/${analysisId}/comments`);

export const addComment = async (
    analysisId: string,
    content: string
): Promise<CommentResponse> =>
    post(`${base}/analyses/${analysisId}/comments`, { content });

export const updateComment = async (
    analysisId: string,
    commentId: string,
    content: string
): Promise<CommentResponse> =>
    put(`${base}/analyses/${analysisId}/comments/${commentId}`, { content });

export const deleteComment = async (
    analysisId: string,
    commentId: string
): Promise<void> =>
    del(`${base}/analyses/${analysisId}/comments/${commentId}`);

export const applyManualCorrection = async (
    analysisId: string,
    payload: {
        correctedClass: number;
        notes?: string;
    }
): Promise<AnalysisResponse> =>
    post(`${base}/analyses/${analysisId}/correction`, payload);

export const fetchClinicalCatalogs = async (): Promise<ClinicalCatalogsResponse> =>
    get(`${base}/clinical-catalogs`);

export const fetchClinicalFollowUp = async (
    analysisId: string
): Promise<ClinicalFollowUpResponse | null> =>
    get(`${base}/analyses/${analysisId}/follow-up`);

export const saveClinicalFollowUp = async (
    analysisId: string,
    payload: UpsertClinicalFollowUpRequest
): Promise<ClinicalFollowUpResponse> =>
    put(`${base}/analyses/${analysisId}/follow-up`, payload);

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
