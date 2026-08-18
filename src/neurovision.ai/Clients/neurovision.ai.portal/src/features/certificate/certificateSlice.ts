import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import {
    deleteCertificate,
    getCertificates,
    uploadCertificate,
} from "./certificate.service";
import {
    CertificateResponse,
    UploadCertificateRequest,
} from "./certificate.types";

interface CertificateState {
    certificates: CertificateResponse[];
    totalCount: number;
    loading: boolean;
    error: string | null;
}

const initialState: CertificateState = {
    certificates: [],
    totalCount: 0,
    loading: false,
    error: null,
};

const toErrorMessage = (err: unknown) => {
    if (err instanceof Error) return err.message;
    if (typeof err === "string") return err;
    return "An unknown error occurred";
};

export const fetchCertificates = createAsyncThunk(
    "certificates/fetchAll",
    async (
        request: { pageIndex: number; pageSize: number },
        { rejectWithValue }
    ) => {
        try {
            return await getCertificates(request.pageIndex, request.pageSize);
        } catch (err: unknown) {
            return rejectWithValue(toErrorMessage(err));
        }
    }
);

export const createCertificate = createAsyncThunk(
    "certificates/create",
    async (request: UploadCertificateRequest, { rejectWithValue }) => {
        try {
            return await uploadCertificate(request);
        } catch (err: unknown) {
            return rejectWithValue(toErrorMessage(err));
        }
    }
);

export const removeCertificate = createAsyncThunk(
    "certificates/delete",
    async (id: string, { rejectWithValue }) => {
        try {
            await deleteCertificate(id);
            return id;
        } catch (err: unknown) {
            return rejectWithValue(toErrorMessage(err));
        }
    }
);

const certificateSlice = createSlice({
    name: "certificates",
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchCertificates.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchCertificates.fulfilled, (state, action) => {
                state.loading = false;
                state.certificates = action.payload.data;
                state.totalCount = action.payload.count;
            })
            .addCase(fetchCertificates.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload as string;
            })
            .addCase(createCertificate.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(createCertificate.fulfilled, (state) => {
                state.loading = false;
            })
            .addCase(createCertificate.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload as string;
            })
            .addCase(removeCertificate.pending, (state) => {
                state.loading = true;
            })
            .addCase(removeCertificate.fulfilled, (state, action) => {
                state.loading = false;
                state.certificates = state.certificates.filter(
                    (item) => item.id !== action.payload
                );
                state.totalCount = Math.max(0, state.totalCount - 1);
            })
            .addCase(removeCertificate.rejected, (state, action) => {
                state.loading = false;
                state.error = action.payload as string;
            });
    },
});

export default certificateSlice.reducer;
