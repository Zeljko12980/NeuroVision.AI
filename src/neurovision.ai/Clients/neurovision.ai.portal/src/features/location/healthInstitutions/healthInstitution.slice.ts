import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getHealthInstitutions, getHealthInstitutionById, createHealthInstitution, updateHealthInstitution, deleteHealthInstitution} from "./healthInstitution.Service";
import { HealthInstitutionResponse, HealthInstitutionRequest } from "./healthInstitution.types";

interface HealthInstitutionState {
    items: HealthInstitutionResponse[];
    selected: HealthInstitutionResponse | null;
    totalCount: number;
    loading: boolean;
    error: string | null;
}

const initialState: HealthInstitutionState = { items: [], selected: null, totalCount: 0, loading: false, error: null };

export const fetchHealthInstitutions = createAsyncThunk("healthInstitutions/fetchAll", async (request: { pageIndex: number; pageSize: number; search?: string }, { rejectWithValue }) => {
    try { return await getHealthInstitutions(request.pageIndex, request.pageSize, request.search); } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

export const fetchHealthInstitution = createAsyncThunk("healthInstitutions/fetchById", async (id: number, { rejectWithValue }) => {
    try { return await getHealthInstitutionById(id); } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

export const createNewHealthInstitution = createAsyncThunk("healthInstitutions/create", async (request: HealthInstitutionRequest, { rejectWithValue }) => {
    try { return await createHealthInstitution(request); } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

export const updateExistingHealthInstitution = createAsyncThunk("healthInstitutions/update", async ({ id, request }: { id: number; request: HealthInstitutionRequest }, { rejectWithValue }) => {
    try { await updateHealthInstitution(id, request); return id; } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

export const deleteExistingHealthInstitution = createAsyncThunk("healthInstitutions/delete", async (id: number, { rejectWithValue }) => {
    try { await deleteHealthInstitution(id); return id; } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

const slice = createSlice({
    name: "healthInstitutions", initialState, reducers: { clearSelected(state) { state.selected = null; } }, extraReducers: (builder) => {
        builder.addCase(fetchHealthInstitutions.pending, (state) => { state.loading = true; state.error = null; })
            .addCase(fetchHealthInstitutions.fulfilled, (state, action) => { state.loading = false; state.items = action.payload.data; state.totalCount = action.payload.count; })
            .addCase(fetchHealthInstitutions.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; })
            .addCase(fetchHealthInstitution.pending, (state) => { state.loading = true; })
            .addCase(fetchHealthInstitution.fulfilled, (state, action) => { state.loading = false; state.selected = action.payload; })
            .addCase(fetchHealthInstitution.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; })
            .addCase(createNewHealthInstitution.pending, (state) => { state.loading = true; })
            .addCase(createNewHealthInstitution.fulfilled, (state) => { state.loading = false; })
            .addCase(createNewHealthInstitution.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; })
            .addCase(updateExistingHealthInstitution.pending, (state) => { state.loading = true; })
            .addCase(updateExistingHealthInstitution.fulfilled, (state) => { state.loading = false; })
            .addCase(updateExistingHealthInstitution.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; })
            .addCase(deleteExistingHealthInstitution.pending, (state) => { state.loading = true; })
            .addCase(deleteExistingHealthInstitution.fulfilled, (state, action) => { state.loading = false; state.items = state.items.filter(x => x.id !== action.payload); state.totalCount--; })
            .addCase(deleteExistingHealthInstitution.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; });
    }
});

export const { clearSelected } = slice.actions;
export default slice.reducer;