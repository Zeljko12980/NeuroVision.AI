import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import {
    AdminUserDto,
    createAdministratorRequest,
    getUsersRequest,
    lockUserRequest,
    unlockUserRequest,
} from "./userService";

interface UserAdminState {
    users: AdminUserDto[];
    loading: boolean;
    error: string | null;
    pageIndex: number;
    pageSize: number;
    totalCount: number;
}

const initialState: UserAdminState = {
    users: [],
    loading: false,
    error: null,
    pageIndex: 0,
    pageSize: 10,
    totalCount: 0,
};

export const fetchUsers = createAsyncThunk(
    "users/fetchUsers",
    async ({
        pageIndex,
        pageSize,
        search,
    }: {
        pageIndex: number;
        pageSize: number;
        search?: string;
    }) => {
        return await getUsersRequest(pageIndex, pageSize, search);
    }
);

export const createAdministrator = createAsyncThunk(
    "users/createAdministrator",
    async (data: { userName: string; email: string }, { rejectWithValue }) => {
        try {
            return await createAdministratorRequest(data);
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : "Failed to create administrator";
            return rejectWithValue(message);
        }
    }
);

export const unlockUser = createAsyncThunk(
    "users/unlockUser",
    async (userId: string, { rejectWithValue }) => {
        try {
            await unlockUserRequest(userId);
            return userId;
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : "Failed to unlock user";
            return rejectWithValue(message);
        }
    }
);

export const lockUser = createAsyncThunk(
    "users/lockUser",
    async (userId: string, { rejectWithValue }) => {
        try {
            await lockUserRequest(userId);
            return userId;
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : "Failed to lock user";
            return rejectWithValue(message);
        }
    }
);

const userSlice = createSlice({
    name: "users",
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(fetchUsers.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchUsers.fulfilled, (state, action) => {
                state.loading = false;
                state.users = action.payload.data;
                state.pageIndex = action.payload.pageIndex;
                state.pageSize = action.payload.pageSize;
                state.totalCount = action.payload.count;
            })
            .addCase(fetchUsers.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || "Error loading users";
            })
            .addCase(unlockUser.fulfilled, (state, action) => {
                const user = state.users.find((item) => item.id === action.payload);
                if (user) {
                    user.isLockedOut = false;
                    user.lockoutEnd = null;
                }
            })
            .addCase(lockUser.fulfilled, (state, action) => {
                const user = state.users.find((item) => item.id === action.payload);
                if (user) {
                    user.isLockedOut = true;
                }
            });
    },
});

export default userSlice.reducer;
