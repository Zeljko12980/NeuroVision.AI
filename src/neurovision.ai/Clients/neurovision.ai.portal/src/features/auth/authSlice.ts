import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { loginRequest, confirm2FARequest, resend2FARequest, confirmEmailRequest, setPasswordRequest } from "./authService";
interface AuthState {
    token: string | null;
    email: string | null;
    requires2FA: boolean;

    loading: boolean;
    resendLoading: boolean;
    confirmEmailLoading: boolean;
    confirmEmailSuccess: boolean;
    
    error: string | null;
    resendMessage: string | null;

    setPasswordLoading: boolean;
    setPasswordSuccess: boolean;
}

const initialState: AuthState = {
    token: localStorage.getItem("token"),
    email: null,
    requires2FA: false,

    loading: false,
    resendLoading: false,

    error: null,
    resendMessage: null,
    confirmEmailLoading: false,
    confirmEmailSuccess: false,
    setPasswordLoading: false,
    setPasswordSuccess: false,
};

export const setPassword = createAsyncThunk<
    { message: string },
    { email: string; token: string; password: string },
    { rejectValue: string }
>(
    "auth/setPassword",
    async ({ email, token, password }, thunkAPI) => {
        try {
            const response = await setPasswordRequest({
                email,
                token,
                password,
            });

            return response;
        } catch (error: any) {
            const data = error.response?.data;
            if (data?.detail) return thunkAPI.rejectWithValue(data.detail);
            return thunkAPI.rejectWithValue("Failed to set password.");
        }
    }
);

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

export const confirmEmail = createAsyncThunk<
    { isConfirmed: boolean },
    { email: string; token: string },
    { rejectValue: string }
>(
    "auth/confirmEmail",
    async ({ email, token }, thunkAPI) => {
        try {
            console.log("Confirming email with:", { email, token });
            const response = await confirmEmailRequest(email, token);

            if (!response.isConfirmed) {
                return thunkAPI.rejectWithValue("Email confirmation failed.");
            }

            return response;
        } catch (error: any) {
            const data = error.response?.data;
            if (data?.detail) return thunkAPI.rejectWithValue(data.detail);
            return thunkAPI.rejectWithValue("Failed to confirm email.");
        }
    }
);



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
        builder.addCase(confirmEmail.pending, (state) => {
            state.confirmEmailLoading = true;
            state.error = null;
            state.confirmEmailSuccess = false;
            });

       builder.addCase(confirmEmail.fulfilled, (state) => {
                state.confirmEmailLoading = false;
                state.confirmEmailSuccess = true;
            });

       builder.addCase(confirmEmail.rejected, (state, action: any) => {
                state.confirmEmailLoading = false;
                state.error = action.payload;
                state.confirmEmailSuccess = false;
       });

        builder.addCase(setPassword.pending, (state) => {
            state.setPasswordLoading = true;
            state.error = null;
            state.setPasswordSuccess = false;
        });

        builder.addCase(setPassword.fulfilled, (state) => {
            state.setPasswordLoading = false;
            state.setPasswordSuccess = true;
        });

        builder.addCase(setPassword.rejected, (state, action: any) => {
            state.setPasswordLoading = false;
            state.error = action.payload;
            state.setPasswordSuccess = false;
        });
    },
});

export const { logout } = authSlice.actions;
export default authSlice.reducer;