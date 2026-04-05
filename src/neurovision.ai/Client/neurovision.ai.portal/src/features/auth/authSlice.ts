import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { loginRequest, confirm2FARequest, resend2FARequest } from "./authService";

interface AuthState {
    token: string | null;
    email: string | null;
    requires2FA: boolean;

    loading: boolean;
    resendLoading: boolean;

    error: string | null;
    resendMessage: string | null;
}

const initialState: AuthState = {
    token: localStorage.getItem("token"),
    email: null,
    requires2FA: false,

    loading: false,
    resendLoading: false,

    error: null,
    resendMessage: null,
};


export const login = createAsyncThunk(
    "auth/login",
    async (
        { email, password }: { email: string; password: string },
        thunkAPI
    ) => {
        try {
            const response = await loginRequest({ email, password });
            return response;
        } catch (error: any) {
            //const detail = error.response?.data?.detail;

             return thunkAPI.rejectWithValue(error.response?.data?.detail);


            if (error.response?.status === 401) return thunkAPI.rejectWithValue("Incorrect email or password.");
            if (error.response?.status === 403) return thunkAPI.rejectWithValue("Access denied.");

            return thunkAPI.rejectWithValue("Unexpected error.");
        }
    }
);

export const verify2FA = createAsyncThunk<
    { token: string },
    { email: string; code: string },
    { rejectValue: string }
>("auth/verify2FA", async ({ email, code }, thunkAPI) => {
    try {
        const response = await confirm2FARequest({ email, code });

        if (!response.token) {
            return thunkAPI.rejectWithValue(response.message || "Invalid 2FA code.");
        }

        return response;
    } catch (error: any) {
        const data = error.response?.data;
        if (data?.detail) return thunkAPI.rejectWithValue(data.detail);
        return thunkAPI.rejectWithValue("Failed to verify 2FA.");
    }
});

export const resend2FA = createAsyncThunk<
    { message?: string },
    { email: string },
    { rejectValue: string }
>("auth/resend2FA", async ({ email }, thunkAPI) => {
    try {
        const response = await resend2FARequest({ email });
        return response;
    } catch (error: any) {
        const data = error.response?.data;
        if (data?.detail) return thunkAPI.rejectWithValue(data.detail);
        return thunkAPI.rejectWithValue("Failed to resend code.");
    }
});



const authSlice = createSlice({
    name: "auth",
    initialState,
    reducers: {
        logout: (state) => {
            state.token = null;
            state.email = null;
            state.requires2FA = false;
            state.error = null;
            state.resendMessage = null;

            localStorage.removeItem("token");
            window.location.href = "/signin";
        },
    },
    extraReducers: (builder) => {
        builder.addCase(login.pending, (state) => {
            state.loading = true;
            state.error = null;
        });
        builder.addCase(login.fulfilled, (state, action) => {
            state.loading = false;
            state.requires2FA = true;
            state.email = action.payload.email;
            state.resendMessage = null;
        });
        builder.addCase(login.rejected, (state, action: any) => {
            state.loading = false;
            state.error = action.payload;
        });

        builder.addCase(verify2FA.pending, (state) => {
            state.loading = true;
            state.error = null;
        });
        builder.addCase(verify2FA.fulfilled, (state, action) => {
            state.loading = false;
            state.requires2FA = false;
            state.token = action.payload.token;
            state.resendMessage = null;

            localStorage.setItem("token", action.payload.token);
        });
        builder.addCase(verify2FA.rejected, (state, action: any) => {
            state.loading = false;
            state.error = action.payload;
        });

        builder.addCase(resend2FA.pending, (state) => {
            state.resendLoading = true;
            state.error = null;
            state.resendMessage = null;
        });
        builder.addCase(resend2FA.fulfilled, (state, action) => {
            state.resendLoading = false;
            state.resendMessage = action.payload?.message || "Code resent successfully.";
        });
        builder.addCase(resend2FA.rejected, (state, action: any) => {
            state.resendLoading = false;
            state.error = action.payload;
        });
    },
});

export const { logout } = authSlice.actions;
export default authSlice.reducer;