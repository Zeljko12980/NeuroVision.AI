import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getGovernmentTypes, getGovernmentTypeByCode, createGovernmentType, updateGovernmentType, deleteGovernmentType, GovernmentTypeRequest, GovernmentTypeResponse } from "./governmentTypeService";

interface GovernmentTypeState {
    items: GovernmentTypeResponse[];
    selected: GovernmentTypeResponse | null;
    totalCount: number;
    loading: boolean;
    error: string | null;
}

const initialState: GovernmentTypeState = { items: [], selected: null, totalCount: 0, loading: false, error: null };

export const fetchGovernmentTypes = createAsyncThunk("governmentTypes/fetchAll", async (request: { pageIndex: number; pageSize: number; search?: string }, { rejectWithValue }) => {
    try { return await getGovernmentTypes(request.pageIndex, request.pageSize, request.search); } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

export const fetchGovernmentTypeByCode = createAsyncThunk("governmentTypes/fetchByCode", async (code: string, { rejectWithValue }) => {
    try { return await getGovernmentTypeByCode(code); } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

export const createNewGovernmentType = createAsyncThunk("governmentTypes/create", async (request: GovernmentTypeRequest, { rejectWithValue }) => {
    try { return await createGovernmentType(request); } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

export const updateExistingGovernmentType = createAsyncThunk("governmentTypes/update", async ({ code, request }: { code: string; request: GovernmentTypeRequest }, { rejectWithValue }) => {
    try { await updateGovernmentType(code, request); return code; } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

export const deleteExistingGovernmentType = createAsyncThunk("governmentTypes/delete", async (code: string, { rejectWithValue }) => {
    try { await deleteGovernmentType(code); return code; } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

const slice = createSlice({ 
    name: "governmentTypes", 
    initialState, 
    reducers: { clearSelected(state) { state.selected = null; } }, 
    extraReducers: (builder) => {
        builder.addCase(fetchGovernmentTypes.pending, (state) => { state.loading = true; state.error = null; })
        .addCase(fetchGovernmentTypes.fulfilled, (state, action) => { state.loading = false; state.items = action.payload.data; state.totalCount = action.payload.count; })
        .addCase(fetchGovernmentTypes.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; })
        .addCase(fetchGovernmentTypeByCode.pending, (state) => { state.loading = true; })
        .addCase(fetchGovernmentTypeByCode.fulfilled, (state, action) => { state.loading = false; state.selected = action.payload; })
        .addCase(fetchGovernmentTypeByCode.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; })
        .addCase(createNewGovernmentType.pending, (state) => { state.loading = true; })
        .addCase(createNewGovernmentType.fulfilled, (state) => { state.loading = false; })
        .addCase(createNewGovernmentType.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; })
        .addCase(updateExistingGovernmentType.pending, (state) => { state.loading = true; })
        .addCase(updateExistingGovernmentType.fulfilled, (state) => { state.loading = false; })
        .addCase(updateExistingGovernmentType.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; })
        .addCase(deleteExistingGovernmentType.pending, (state) => { state.loading = true; })
        .addCase(deleteExistingGovernmentType.fulfilled, (state, action) => { state.loading = false; state.items = state.items.filter(x => x.code !== action.payload); state.totalCount--; })
        .addCase(deleteExistingGovernmentType.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; });
    }
});


export const selectGovernmentTypes = (
    state: {
        governmentTypes: GovernmentTypeState
    }
) => state.governmentTypes.items;
export const { clearSelected } = slice.actions;
export default slice.reducer;
