import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import {
    createPdfTemplate,
    deletePdfTemplate,
    getPdfTemplateById,
    getPdfTemplates,
    updatePdfTemplate,
} from "./pdf.service";
import { PdfTemplateResponse, PdfTemplateRequest } from "./pdf.types";

interface PdfTemplateState {
    templates: PdfTemplateResponse[];
    selectedTemplate: PdfTemplateResponse | null;
    totalCount: number;
    loading: boolean;
    error: string | null;
}

const initialState: PdfTemplateState = {
    templates: [],
    selectedTemplate: null,
    totalCount: 0,
    loading: false,
    error: null,
};

export const fetchPdfTemplates = createAsyncThunk(
    "pdfTemplates/fetchAll",
    async (
        request: {
            pageIndex: number;
            pageSize: number;
            search?: string;
        },
        { rejectWithValue }
    ) => {
        try {
            console.log("Fetching PDF templates with request:", request);
            return await getPdfTemplates(
                request.pageIndex,
                request.pageSize,
                request.search
            );
        } catch (err: unknown) {
            let message = "An unknown error occurred";
            if (err instanceof Error) message = err.message;
            else if (typeof err === "string") message = err;
            return rejectWithValue(message);
        }
    }
);
            
export const fetchPdfTemplateById = createAsyncThunk(
    "pdfTemplates/fetchById",
    async (id: string, { rejectWithValue }) => {
        try {
            return await getPdfTemplateById(id);
        } catch (err: unknown) {
            let message = "An unknown error occurred";
            if (err instanceof Error) message = err.message;
            else if (typeof err === "string") message = err;
            return rejectWithValue(message);
        }
    }
);

export const createTemplate = createAsyncThunk(
    "pdfTemplates/create",
    async (request: PdfTemplateRequest, { rejectWithValue }) => {
        try {
            return await createPdfTemplate(request);
        } catch (err: unknown) {
            let message = "An unknown error occurred";
            if (err instanceof Error) message = err.message;
            else if (typeof err === "string") message = err;
            return rejectWithValue(message);
        }
    }
);

export const updateTemplate = createAsyncThunk(
    "pdfTemplates/update",
    async (
        {
            id,
            request,
        }: {
            id: string;
            request: PdfTemplateRequest;
        },
        { rejectWithValue }
    ) => {
        try {
            await updatePdfTemplate(id, request);
            return id;
        } catch (err: unknown) {
            let message = "An unknown error occurred";
            if (err instanceof Error) message = err.message;
            else if (typeof err === "string") message = err;
            return rejectWithValue(message);
        }
    }
);

export const deleteTemplate = createAsyncThunk(
    "pdfTemplates/delete",
    async (id: string, { rejectWithValue }) => {
        try {
            await deletePdfTemplate(id);
            return id;
        } catch (err: unknown) {
            let message = "An unknown error occurred";
            if (err instanceof Error) message = err.message;
            else if (typeof err === "string") message = err;
            return rejectWithValue(message);
        }
    }
);

const pdfTemplateSlice = createSlice({
    name: "pdfTemplates",
    initialState,
    reducers: {
        clearSelectedTemplate(state) {
            state.selectedTemplate = null;
        },
    },
    extraReducers: (builder) => {
        builder

           
            .addCase(fetchPdfTemplates.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchPdfTemplates.fulfilled, (state, action) => {
                state.loading = false;
                state.templates = action.payload.data;
                state.totalCount = action.payload.count;
            })
            .addCase(fetchPdfTemplates.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload as string;
            })

       
            .addCase(fetchPdfTemplateById.pending, (state) => {
                state.loading = true;
            })
            .addCase(fetchPdfTemplateById.fulfilled, (state, action) => {
                state.loading = false;
                state.selectedTemplate = action.payload;
            })
            .addCase(fetchPdfTemplateById.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload as string;
            })

       
            .addCase(createTemplate.pending, (state) => {
                state.loading = true;
            })
            .addCase(createTemplate.fulfilled, (state) => {
                state.loading = false;
            })
            .addCase(createTemplate.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload as string;
            })

           
            .addCase(updateTemplate.pending, (state) => {
                state.loading = true;
            })
            .addCase(updateTemplate.fulfilled, (state) => {
                state.loading = false;
            })
            .addCase(updateTemplate.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload as string;
            })

      
            .addCase(deleteTemplate.pending, (state) => {
                state.loading = true;
            })
            .addCase(deleteTemplate.fulfilled, (state, action) => {
                state.loading = false;
                state.templates = state.templates.filter(
                    (x) => x.id !== action.payload
                );
                state.totalCount--;
            })
            .addCase(deleteTemplate.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload as string;
            });
    },
});

export const { clearSelectedTemplate } = pdfTemplateSlice.actions;

export default pdfTemplateSlice.reducer;