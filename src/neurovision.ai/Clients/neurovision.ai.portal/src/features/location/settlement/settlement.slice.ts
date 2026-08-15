import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import { getSettlements, getSettlementByKey, createSettlement, updateSettlement, deleteSettlement, SettlementRequest, SettlementResponse } from "./settlement.service";

interface SettlementState {
    items: SettlementResponse[];
    selected: SettlementResponse | null;
    totalCount: number;
    loading: boolean;
    error: string | null;
}

const initialState: SettlementState = { items: [], selected: null, totalCount: 0, loading: false, error: null };

export const fetchSettlements = createAsyncThunk("settlements/fetchAll", async (request: { pageIndex: number; pageSize: number; search?: string }, { rejectWithValue }) => {
    try { return await getSettlements(request.pageIndex, request.pageSize, request.search); } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

export const fetchSettlement = createAsyncThunk("settlements/fetchByKey", async ({ countryCode, code }: { countryCode: string; code: number }, { rejectWithValue }) => {
    try { return await getSettlementByKey(countryCode, code); } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

export const createNewSettlement = createAsyncThunk("settlements/create", async (request: SettlementRequest, { rejectWithValue }) => {
    try { return await createSettlement(request); } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

export const updateExistingSettlement = createAsyncThunk("settlements/update", async ({ countryCode, code, request }: { countryCode: string; code: number; request: SettlementRequest }, { rejectWithValue }) => {
    try { await updateSettlement(countryCode, code, request); return { countryCode, code }; } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

export const deleteExistingSettlement = createAsyncThunk("settlements/delete", async ({ countryCode, code }: { countryCode: string; code: number }, { rejectWithValue }) => {
    try { await deleteSettlement(countryCode, code); return { countryCode, code }; } catch (err: unknown) { let message = "An unknown error occurred"; if (err instanceof Error) message = err.message; else if (typeof err === "string") message = err; return rejectWithValue(message); }
});

const slice = createSlice({ name: "settlements", initialState, reducers: { clearSelected(state) { state.selected = null; } }, extraReducers: (builder) => {
    builder.addCase(fetchSettlements.pending, (state) => { state.loading = true; state.error = null; })
    .addCase(fetchSettlements.fulfilled, (state, action) => { state.loading = false; state.items = action.payload.data; state.totalCount = action.payload.count; })
    .addCase(fetchSettlements.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; })
    .addCase(fetchSettlement.pending, (state) => { state.loading = true; })
    .addCase(fetchSettlement.fulfilled, (state, action) => { state.loading = false; state.selected = action.payload; })
    .addCase(fetchSettlement.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; })
    .addCase(createNewSettlement.pending, (state) => { state.loading = true; })
    .addCase(createNewSettlement.fulfilled, (state) => { state.loading = false; })
    .addCase(createNewSettlement.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; })
    .addCase(updateExistingSettlement.pending, (state) => { state.loading = true; })
    .addCase(updateExistingSettlement.fulfilled, (state) => { state.loading = false; })
    .addCase(updateExistingSettlement.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; })
    .addCase(deleteExistingSettlement.pending, (state) => { state.loading = true; })
    .addCase(deleteExistingSettlement.fulfilled, (state, action) => { state.loading = false; state.items = state.items.filter(x => !(x.countryCode === action.payload.countryCode && x.code === action.payload.code)); state.totalCount--; })
    .addCase(deleteExistingSettlement.rejected, (state, action) => { state.loading = false; state.error = action.payload as string; });
}
});


export const { clearSelected } = slice.actions;
export default slice.reducer;
