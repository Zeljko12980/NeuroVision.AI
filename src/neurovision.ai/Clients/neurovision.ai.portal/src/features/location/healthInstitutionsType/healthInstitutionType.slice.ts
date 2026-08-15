import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getHealthInstitutionTypes, getHealthInstitutionTypeByCode, createHealthInstitutionType, updateHealthInstitutionType, deleteHealthInstitutionType } from "./healthInstitutionType.service";
import { HealthInstitutionTypeResponse, HealthInstitutionTypeRequest } from "./healthInstitutionType.type";

interface HealthInstitutionTypeState {
    items: HealthInstitutionTypeResponse[];
    selected: HealthInstitutionTypeResponse | null;
    totalCount: number;
    loading: boolean;
    error: string | null;
}

const initialState: HealthInstitutionTypeState = { items: [], selected: null, totalCount: 0, loading: false, error: null };

export const fetchHealthInstitutionTypes = createAsyncThunk("healthInstitutionTypes/fetchAll", async (request: { pageIndex: number; pageSize: number; search?: string }, { rejectWithValue }) => {
    try { return await getHealthInstitutionTypes(request.pageIndex, request.pageSize, request.search); } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

export const fetchHealthInstitutionType = createAsyncThunk("healthInstitutionTypes/fetchByCode", async (code: string, { rejectWithValue }) => {
    try { return await getHealthInstitutionTypeByCode(code); } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

export const createNewHealthInstitutionType = createAsyncThunk("healthInstitutionTypes/create", async (request: HealthInstitutionTypeRequest, { rejectWithValue }) => {
    try { return await createHealthInstitutionType(request); } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

export const updateExistingHealthInstitutionType = createAsyncThunk("healthInstitutionTypes/update", async ({ code, request }: { code: string; request: HealthInstitutionTypeRequest }, { rejectWithValue }) => {
    try { await updateHealthInstitutionType(code, request); return code; } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

export const deleteExistingHealthInstitutionType = createAsyncThunk("healthInstitutionTypes/delete", async (code: string, { rejectWithValue }) => {
    try { await deleteHealthInstitutionType(code); return code; } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

const slice = createSlice({ name: "healthInstitutionTypes", initialState, reducers: { clearSelected(state) { state.selected = null; } }, extraReducers: (builder) => {
    builder.addCase(fetchHealthInstitutionTypes.pending, (state) => { state.loading = true; state.error = null; })
    .addCase(fetchHealthInstitutionTypes.fulfilled, (state, action) => { state.loading = false; state.items = action.payload.data; state.totalCount = action.payload.count; })
    .addCase(fetchHealthInstitutionTypes.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; })
    .addCase(fetchHealthInstitutionType.pending, (state) => { state.loading = true; })
    .addCase(fetchHealthInstitutionType.fulfilled, (state, action) => { state.loading = false; state.selected = action.payload; })
    .addCase(fetchHealthInstitutionType.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; })
    .addCase(createNewHealthInstitutionType.pending, (state) => { state.loading = true; })
    .addCase(createNewHealthInstitutionType.fulfilled, (state) => { state.loading = false; })
    .addCase(createNewHealthInstitutionType.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; })
    .addCase(updateExistingHealthInstitutionType.pending, (state) => { state.loading = true; })
    .addCase(updateExistingHealthInstitutionType.fulfilled, (state) => { state.loading = false; })
    .addCase(updateExistingHealthInstitutionType.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; })
    .addCase(deleteExistingHealthInstitutionType.pending, (state) => { state.loading = true; })
    .addCase(deleteExistingHealthInstitutionType.fulfilled, (state, action) => { state.loading = false; state.items = state.items.filter(x => x.code !== action.payload); state.totalCount--; })
    .addCase(deleteExistingHealthInstitutionType.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; });
} });

export const { clearSelected } = slice.actions;
export default slice.reducer;
