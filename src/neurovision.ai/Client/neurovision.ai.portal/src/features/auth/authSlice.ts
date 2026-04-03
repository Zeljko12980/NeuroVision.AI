import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { loginRequest } from "./authService";
import { confirm2FARequest } from "./authService"; 

interface AuthState {
    token: string | null;
    email: string | null;
    requires2FA: boolean;
    loading: boolean;
    error: string | null;
}

const initialState: AuthState = {
    token: localStorage.getItem("token"),
    email: null,
    requires2FA: false,
    loading: false,
    error: null,
};


export const login = createAsyncThunk(
    "auth/login",
    async (
        { email, password }: { email: string; password: string },
        thunkAPI
    ) => {
        try {
            const data = await loginRequest({ email, password });
            return data;
        } catch (error: any) {
            if (error.response) {
                const status = error.response.status;
                if (status === 401) {
                    return thunkAPI.rejectWithValue("Incorrect email or password.");
                } else if (status === 403) {
                    return thunkAPI.rejectWithValue("Access denied. Please check your account.");
                } else {
                    return thunkAPI.rejectWithValue("Something went wrong. Please try again.");
                }
            } else if (error.request) {
                return thunkAPI.rejectWithValue(
                    "Cannot connect to server. Please check your internet connection."
                );
            } else {
                return thunkAPI.rejectWithValue("An unexpected error occurred. Please try again.");
            }
        }
    }
);


export const verify2FA = createAsyncThunk(
    "auth/verify2FA",
    async (
        { email, code }: { email: string; code: string },
        thunkAPI
    ) => {
        try {
            const response = await confirm2FARequest({ email, code });

            if (!response.token) {
                return thunkAPI.rejectWithValue(response.message || "Invalid 2FA code.");
            }

            return response; 
        } catch (error: any) {
            if (error.response && error.response.data?.message) {
                return thunkAPI.rejectWithValue(error.response.data.message);
            }
            return thunkAPI.rejectWithValue("Failed to verify 2FA. Please try again.");
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
            localStorage.setItem("token", action.payload.token);
        });
        builder.addCase(verify2FA.rejected, (state, action: any) => {
            state.loading = false;
            state.error = action.payload;
        });
    },
});

export const { logout } = authSlice.actions;
export default authSlice.reducer;