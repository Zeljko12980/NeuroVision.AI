import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import {
    cancelAppointmentRequest,
    createAppointmentRequest,
    getAppointmentCatalogs,
    getAppointments,
    rescheduleAppointmentRequest,
} from "./appointmentService";
import {
    AppointmentCatalogsResponse,
    AppointmentRangeQuery,
    AppointmentResponse,
    CreateAppointmentRequest,
    RescheduleAppointmentRequest,
} from "./appointment.types";

interface AppointmentState {
    items: AppointmentResponse[];
    catalogs: AppointmentCatalogsResponse | null;
    loading: boolean;
    saving: boolean;
    error: string | null;
}

const initialState: AppointmentState = {
    items: [],
    catalogs: null,
    loading: false,
    saving: false,
    error: null,
};

const toErrorMessage = (err: unknown, fallback: string) => {
    if (typeof err === "string" && err.trim()) return err;
    if (err instanceof Error && err.message.trim()) return err.message;
    return fallback;
};

export const fetchAppointments = createAsyncThunk<
    AppointmentResponse[],
    AppointmentRangeQuery,
    { rejectValue: string }
>("appointment/fetchRange", async (query, { rejectWithValue }) => {
    try {
        return await getAppointments(query);
    } catch (err: unknown) {
        return rejectWithValue(toErrorMessage(err, "Failed to load appointments"));
    }
});

export const fetchAppointmentCatalogs = createAsyncThunk<
    AppointmentCatalogsResponse,
    void,
    { rejectValue: string }
>("appointment/fetchCatalogs", async (_, { rejectWithValue }) => {
    try {
        return await getAppointmentCatalogs();
    } catch (err: unknown) {
        return rejectWithValue(toErrorMessage(err, "Failed to load appointment catalogs"));
    }
});

export const createAppointment = createAsyncThunk<
    AppointmentResponse,
    CreateAppointmentRequest,
    { rejectValue: string }
>("appointment/create", async (payload, { rejectWithValue }) => {
    try {
        return await createAppointmentRequest(payload);
    } catch (err: unknown) {
        return rejectWithValue(toErrorMessage(err, "Failed to create appointment"));
    }
});

export const rescheduleAppointment = createAsyncThunk<
    AppointmentResponse,
    { id: string; payload: RescheduleAppointmentRequest },
    { rejectValue: string }
>("appointment/reschedule", async ({ id, payload }, { rejectWithValue }) => {
    try {
        return await rescheduleAppointmentRequest(id, payload);
    } catch (err: unknown) {
        return rejectWithValue(toErrorMessage(err, "Failed to reschedule appointment"));
    }
});

export const cancelAppointment = createAsyncThunk<
    AppointmentResponse,
    string,
    { rejectValue: string }
>("appointment/cancel", async (id, { rejectWithValue }) => {
    try {
        return await cancelAppointmentRequest(id);
    } catch (err: unknown) {
        return rejectWithValue(toErrorMessage(err, "Failed to cancel appointment"));
    }
});

const appointmentSlice = createSlice({
    name: "appointment",
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchAppointments.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchAppointments.fulfilled, (state, action) => {
                state.loading = false;
                state.items = action.payload;
            })
            .addCase(fetchAppointments.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload ?? "Failed to load appointments";
            })
            .addCase(fetchAppointmentCatalogs.fulfilled, (state, action) => {
                state.catalogs = action.payload;
            })
            .addCase(createAppointment.pending, (state) => {
                state.saving = true;
                state.error = null;
            })
            .addCase(createAppointment.fulfilled, (state, action) => {
                state.saving = false;
                state.items = [...state.items.filter((item) => item.id !== action.payload.id), action.payload];
            })
            .addCase(createAppointment.rejected, (state, action) => {
                state.saving = false;
                state.error = action.payload ?? "Failed to create appointment";
            })
            .addCase(rescheduleAppointment.pending, (state) => {
                state.saving = true;
                state.error = null;
            })
            .addCase(rescheduleAppointment.fulfilled, (state, action) => {
                state.saving = false;
                state.items = state.items.map((item) =>
                    item.id === action.payload.id ? action.payload : item
                );
            })
            .addCase(rescheduleAppointment.rejected, (state, action) => {
                state.saving = false;
                state.error = action.payload ?? "Failed to reschedule appointment";
            })
            .addCase(cancelAppointment.pending, (state) => {
                state.saving = true;
                state.error = null;
            })
            .addCase(cancelAppointment.fulfilled, (state, action) => {
                state.saving = false;
                state.items = state.items.map((item) =>
                    item.id === action.payload.id ? action.payload : item
                );
            })
            .addCase(cancelAppointment.rejected, (state, action) => {
                state.saving = false;
                state.error = action.payload ?? "Failed to cancel appointment";
            });
    },
});

export default appointmentSlice.reducer;
