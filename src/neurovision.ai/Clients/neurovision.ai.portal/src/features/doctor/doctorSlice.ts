import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { createDoctorRequest, CreateDoctorRequest } from "./doctorService.ts";

interface DoctorState {
    loading: boolean;
    success: boolean;
    doctorId: string | null;
    error: string | null;
}

const initialState: DoctorState = {
    loading: false,
    success: false,
    doctorId: null,
    error: null,
};


export const createDoctor = createAsyncThunk<
    string,
    CreateDoctorRequest,
    { rejectValue: string }
>(
    "doctor/create",
    async (data, { rejectWithValue }) => {
        try {
            const response = await createDoctorRequest(data);
            return response.id;
        } catch (err: any) {
            return rejectWithValue(
                err?.response?.data?.message || "Failed to create doctor"
            );
        }
    }
);


const doctorSlice = createSlice({
    name: "doctor",
    initialState,
    reducers: {
        resetDoctorState: (state) => {
            state.loading = false;
            state.success = false;
            state.doctorId = null;
            state.error = null;
        },
    },
    extraReducers: (builder) => {
        builder
            .addCase(createDoctor.pending, (state) => {
                state.loading = true;
                state.success = false;
                state.error = null;
            })
            .addCase(createDoctor.fulfilled, (state, action) => {
                state.loading = false;
                state.success = true;
                state.doctorId = action.payload;
            })
            .addCase(createDoctor.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload || "Error occurred";
            });
    },
});

export const { resetDoctorState } = doctorSlice.actions;
export default doctorSlice.reducer;