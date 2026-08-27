import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { getSystemHealth, type SystemHealthResponse } from "./healthService";

interface HealthState {
    data: SystemHealthResponse | null;
    loading: boolean;
    error: string | null;
}

const initialState: HealthState = {
    data: null,
    loading: false,
    error: null,
};

export const fetchSystemHealth = createAsyncThunk(
    "health/fetchSystemHealth",
    async (
        { pageIndex = 0, pageSize = 10 }: { pageIndex?: number; pageSize?: number },
        { rejectWithValue }
    ) => {
        try {
            return await getSystemHealth(pageIndex, pageSize);
        } catch (error: any) {
            return rejectWithValue(
                error.response?.data?.message ?? "Failed to load system health"
            );
        }
    }
);

const healthSlice = createSlice({
    name: "health",
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchSystemHealth.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchSystemHealth.fulfilled, (state, action) => {
                state.loading = false;
                state.data = action.payload;
            })
            .addCase(fetchSystemHealth.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload as string;
            });
    },
});

export default healthSlice.reducer;
