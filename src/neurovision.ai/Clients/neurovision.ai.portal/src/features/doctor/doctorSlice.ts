import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import {
    createDoctorRequest,
    deleteDoctorRequest,
    getDoctorById,
    getDoctors,
} from "./doctorService";
import {
    CreateDoctorRequest,
    DoctorResponse,
    PaginatedDoctorResponse,
} from "./doctor.types";

interface DoctorState {
    items: DoctorResponse[];
    selected: DoctorResponse | null;
    totalCount: number;
    loading: boolean;
    success: boolean;
    doctorId: string | null;
    error: string | null;
}

const initialState: DoctorState = {
    items: [],
    selected: null,
    totalCount: 0,
    loading: false,
    success: false,
    doctorId: null,
    error: null,
};

const toErrorMessage = (err: unknown, fallback: string) => {
    if (typeof err === "string" && err.trim()) return err;
    if (err instanceof Error && err.message.trim()) return err.message;
    return fallback;
};

export const fetchDoctors = createAsyncThunk<
    PaginatedDoctorResponse,
    { pageIndex: number; pageSize: number; search?: string },
    { rejectValue: string }
>("doctor/fetchAll", async (request, { rejectWithValue }) => {
    try {
        return await getDoctors(request.pageIndex, request.pageSize, request.search);
    } catch (err: unknown) {
        return rejectWithValue(toErrorMessage(err, "Failed to load doctors"));
    }
});

export const fetchDoctor = createAsyncThunk<
    DoctorResponse,
    string,
    { rejectValue: string }
>("doctor/fetchById", async (id, { rejectWithValue }) => {
    try {
        return await getDoctorById(id);
    } catch (err: unknown) {
        return rejectWithValue(toErrorMessage(err, "Failed to load doctor"));
    }
});

export const createDoctor = createAsyncThunk<
    DoctorResponse,
    CreateDoctorRequest,
    { rejectValue: string }
>("doctor/create", async (data, { rejectWithValue }) => {
    try {
        return await createDoctorRequest(data);
    } catch (err: unknown) {
        return rejectWithValue(toErrorMessage(err, "Failed to create doctor"));
    }
});

export const deleteExistingDoctor = createAsyncThunk<
    string,
    string,
    { rejectValue: string }
>("doctor/delete", async (id, { rejectWithValue }) => {
    try {
        await deleteDoctorRequest(id);
        return id;
    } catch (err: unknown) {
        return rejectWithValue(toErrorMessage(err, "Failed to delete doctor"));
    }
});

const doctorSlice = createSlice({
    name: "doctor",
    initialState,
    reducers: {
        resetDoctorState: (state) => {
            state.loading = false;
            state.success = false;
            state.doctorId = null;
            state.error = null;
            state.selected = null;
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(fetchDoctors.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchDoctors.fulfilled, (state, action) => {
                state.loading = false;
                state.items = action.payload.data ?? [];
                state.totalCount = action.payload.count ?? 0;
            })
            .addCase(fetchDoctors.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload || "Error occurred";
            })
            .addCase(fetchDoctor.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchDoctor.fulfilled, (state, action) => {
                state.loading = false;
                state.selected = action.payload;
            })
            .addCase(fetchDoctor.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload || "Error occurred";
            })
            .addCase(createDoctor.pending, (state) => {
                state.loading = true;
                state.success = false;
                state.error = null;
            })
            .addCase(createDoctor.fulfilled, (state, action) => {
                state.loading = false;
                state.success = true;
                state.doctorId = action.payload.id;
            })
            .addCase(createDoctor.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload || "Error occurred";
            })
            .addCase(deleteExistingDoctor.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(deleteExistingDoctor.fulfilled, (state, action) => {
                state.loading = false;
                state.items = state.items.filter((item) => item.id !== action.payload);
                state.totalCount = Math.max(0, state.totalCount - 1);
            })
            .addCase(deleteExistingDoctor.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload || "Error occurred";
            });
    },
});

export const { resetDoctorState } = doctorSlice.actions;
export default doctorSlice.reducer;
