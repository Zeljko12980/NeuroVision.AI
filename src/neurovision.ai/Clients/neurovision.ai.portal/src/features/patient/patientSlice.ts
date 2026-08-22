import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import {
    createPatientRequest,
    deletePatientRequest,
    getPatientById,
    getPatients,
} from "./patientService";
import {
    CreatePatientRequest,
    PatientResponse,
    PaginatedPatientResponse,
} from "./patient.types";

interface PatientState {
    items: PatientResponse[];
    selected: PatientResponse | null;
    totalCount: number;
    loading: boolean;
    success: boolean;
    patientId: string | null;
    error: string | null;
}

const initialState: PatientState = {
    items: [],
    selected: null,
    totalCount: 0,
    loading: false,
    success: false,
    patientId: null,
    error: null,
};

const toErrorMessage = (err: unknown, fallback: string) => {
    if (typeof err === "string" && err.trim()) return err;
    if (err instanceof Error && err.message.trim()) return err.message;
    return fallback;
};

export const fetchPatients = createAsyncThunk<
    PaginatedPatientResponse,
    { pageIndex: number; pageSize: number; search?: string },
    { rejectValue: string }
>("patient/fetchAll", async (request, { rejectWithValue }) => {
    try {
        return await getPatients(request.pageIndex, request.pageSize, request.search);
    } catch (err: unknown) {
        return rejectWithValue(toErrorMessage(err, "Failed to load patients"));
    }
});

export const fetchPatient = createAsyncThunk<
    PatientResponse,
    string,
    { rejectValue: string }
>("patient/fetchById", async (id, { rejectWithValue }) => {
    try {
        return await getPatientById(id);
    } catch (err: unknown) {
        return rejectWithValue(toErrorMessage(err, "Failed to load patient"));
    }
});

export const createPatient = createAsyncThunk<
    PatientResponse,
    CreatePatientRequest,
    { rejectValue: string }
>("patient/create", async (data, { rejectWithValue }) => {
    try {
        return await createPatientRequest(data);
    } catch (err: unknown) {
        return rejectWithValue(toErrorMessage(err, "Failed to create patient"));
    }
});

export const deleteExistingPatient = createAsyncThunk<
    string,
    string,
    { rejectValue: string }
>("patient/delete", async (id, { rejectWithValue }) => {
    try {
        await deletePatientRequest(id);
        return id;
    } catch (err: unknown) {
        return rejectWithValue(toErrorMessage(err, "Failed to delete patient"));
    }
});

const patientSlice = createSlice({
    name: "patient",
    initialState,
    reducers: {
        resetPatientState: (state) => {
            state.loading = false;
            state.success = false;
            state.patientId = null;
            state.error = null;
            state.selected = null;
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchPatients.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchPatients.fulfilled, (state, action) => {
                state.loading = false;
                state.items = action.payload.data ?? [];
                state.totalCount = action.payload.count ?? 0;
            })
            .addCase(fetchPatients.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload || "Error occurred";
            })
            .addCase(fetchPatient.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchPatient.fulfilled, (state, action) => {
                state.loading = false;
                state.selected = action.payload;
            })
            .addCase(fetchPatient.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload || "Error occurred";
            })
            .addCase(createPatient.pending, (state) => {
                state.loading = true;
                state.success = false;
                state.error = null;
            })
            .addCase(createPatient.fulfilled, (state, action) => {
                state.loading = false;
                state.success = true;
                state.patientId = action.payload.id;
            })
            .addCase(createPatient.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload || "Error occurred";
            })
            .addCase(deleteExistingPatient.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(deleteExistingPatient.fulfilled, (state, action) => {
                state.loading = false;
                state.items = state.items.filter((item) => item.id !== action.payload);
                state.totalCount = Math.max(0, state.totalCount - 1);
            })
            .addCase(deleteExistingPatient.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload || "Error occurred";
            });
    },
});

export const { resetPatientState } = patientSlice.actions;
export default patientSlice.reducer;
