import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import type {
    AiModelVersionResponse,
    AnalysisResponse,
    AnalysisStatisticsResponse,
    BrainScanResponse,
    PaginatedResponse,
} from "./tumorDetection.types";
import {
    fetchModelVersions,
    fetchScans,
    fetchStatistics,
    searchAnalyses,
    startAnalysis,
    uploadScan,
} from "./tumorDetection.service";

interface TumorDetectionState {
    scans: BrainScanResponse[];
    scansTotal: number;
    analyses: AnalysisResponse[];
    analysesTotal: number;
    statistics: AnalysisStatisticsResponse | null;
    models: AiModelVersionResponse[];
    loading: boolean;
    startingAnalysis: boolean;
    uploading: boolean;
    error: string | null;
}

const initialState: TumorDetectionState = {
    scans: [],
    scansTotal: 0,
    analyses: [],
    analysesTotal: 0,
    statistics: null,
    models: [],
    loading: false,
    startingAnalysis: false,
    uploading: false,
    error: null,
};

export const loadScans = createAsyncThunk<
    PaginatedResponse<BrainScanResponse>,
    { patientId?: string; page?: number; pageSize?: number },
    { rejectValue: string }
>("tumorDetection/loadScans", async (params, { rejectWithValue }) => {
    try {
        return await fetchScans(params.patientId, params.page ?? 1, params.pageSize ?? 10);
    } catch (err) {
        return rejectWithValue(err instanceof Error ? err.message : "Failed to load scans");
    }
});

export const uploadBrainScan = createAsyncThunk<
    BrainScanResponse,
    { patientId: string; uploadedByUserId: string; scanType: "Mri" | "Ct"; file: File },
    { rejectValue: string }
>("tumorDetection/uploadScan", async (payload, { rejectWithValue }) => {
    try {
        return await uploadScan(payload);
    } catch (err) {
        return rejectWithValue(err instanceof Error ? err.message : "Upload failed");
    }
});

export const loadAnalyses = createAsyncThunk<
    PaginatedResponse<AnalysisResponse>,
    { patientId?: string; page?: number; pageSize?: number; archived?: boolean },
    { rejectValue: string }
>("tumorDetection/loadAnalyses", async (params, { rejectWithValue }) => {
    try {
        const response = await searchAnalyses({
            patientId: params.patientId,
            page: params.page ?? 1,
            pageSize: params.pageSize ?? 10,
            archived: params.archived,
        });
        return response;
    } catch (err) {
        return rejectWithValue(err instanceof Error ? err.message : "Failed to load analyses");
    }
});

export const runAnalysis = createAsyncThunk<
    AnalysisResponse,
    { brainScanId: string; requestedByUserId: string },
    { rejectValue: string }
>("tumorDetection/runAnalysis", async (payload, { rejectWithValue }) => {
    try {
        return await startAnalysis(payload.brainScanId, payload.requestedByUserId);
    } catch (err) {
        return rejectWithValue(err instanceof Error ? err.message : "Analysis failed");
    }
});

export const loadStatistics = createAsyncThunk<
    AnalysisStatisticsResponse,
    void,
    { rejectValue: string }
>("tumorDetection/loadStatistics", async (_, { rejectWithValue }) => {
    try {
        return await fetchStatistics();
    } catch (err) {
        return rejectWithValue(err instanceof Error ? err.message : "Failed to load statistics");
    }
});

export const loadModels = createAsyncThunk<
    AiModelVersionResponse[],
    void,
    { rejectValue: string }
>("tumorDetection/loadModels", async (_, { rejectWithValue }) => {
    try {
        return await fetchModelVersions();
    } catch (err) {
        return rejectWithValue(err instanceof Error ? err.message : "Failed to load models");
    }
});

const tumorDetectionSlice = createSlice({
    name: "tumorDetection",
    initialState,
    reducers: {
        clearTumorError(state) {
            state.error = null;
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(loadScans.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(loadScans.fulfilled, (state, action) => {
                state.loading = false;
                state.scans = action.payload.items;
                state.scansTotal = action.payload.total;
            })
            .addCase(loadScans.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload ?? "Failed to load scans";
            })
            .addCase(uploadBrainScan.pending, (state) => {
                state.uploading = true;
            })
            .addCase(uploadBrainScan.fulfilled, (state) => {
                state.uploading = false;
            })
            .addCase(uploadBrainScan.rejected, (state, action) => {
                state.uploading = false;
                state.error = action.payload ?? "Upload failed";
            })
            .addCase(loadAnalyses.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(loadAnalyses.fulfilled, (state, action) => {
                state.loading = false;
                state.analyses = action.payload.items;
                state.analysesTotal = action.payload.total;
            })
            .addCase(loadAnalyses.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload ?? "Failed to load analyses";
            })
            .addCase(runAnalysis.pending, (state) => {
                state.startingAnalysis = true;
            })
            .addCase(runAnalysis.fulfilled, (state, action) => {
                state.startingAnalysis = false;
                state.analyses = [action.payload, ...state.analyses.filter((a) => a.id !== action.payload.id)];
            })
            .addCase(runAnalysis.rejected, (state, action) => {
                state.startingAnalysis = false;
                state.error = action.payload ?? "Analysis failed";
            })
            .addCase(loadStatistics.fulfilled, (state, action) => {
                state.statistics = action.payload;
            })
            .addCase(loadModels.fulfilled, (state, action) => {
                state.models = action.payload;
            });
    },
});

export const { clearTumorError } = tumorDetectionSlice.actions;
export default tumorDetectionSlice.reducer;
