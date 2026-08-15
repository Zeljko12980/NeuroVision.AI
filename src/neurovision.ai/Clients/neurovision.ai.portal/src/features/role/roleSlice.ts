import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import {
    getRolesRequest,
    deleteRoleRequest,
    updateRoleRequest,
    RoleDto,
    createRoleRequest
} from "./roleService";

interface RoleState {
    roles: RoleDto[];
    loading: boolean;
    error: string | null;
    pageIndex: number;
    pageSize: number;
    totalCount: number;
}

const initialState: RoleState = {
    roles: [],
    loading: false,
    error: null,
    pageIndex: 0,
    pageSize: 10,
    totalCount: 0,
};

export const createRole = createAsyncThunk(
    "roles/createRole",
    async (role: {
        roleName: string;
        description?: string;
    }) => {
        return await createRoleRequest(role);
    }
);

export const fetchRoles = createAsyncThunk(
    "roles/fetchRoles",
    async ({ pageIndex, pageSize }: { pageIndex: number; pageSize: number }) => {
        return await getRolesRequest(pageIndex, pageSize);
    }
);


export const deleteRole = createAsyncThunk(
    "roles/deleteRole",
    async (id: string, { rejectWithValue }) => {
        try {
            await deleteRoleRequest(id);
            return id;
        } catch (err: any) {
            return rejectWithValue(err?.message || "Delete failed");
        }
    }
);

export const updateRole = createAsyncThunk(
    "roles/updateRole",
    async (role: {
        id: string;
        roleName: string;
        description?: string;
    }) => {
        return await updateRoleRequest(role);
    }
);

const roleSlice = createSlice({
    name: "roles",
    initialState,
    reducers: {},

    extraReducers: (builder) => {
        builder

            .addCase(fetchRoles.pending, (state) => {
                state.loading = true;
                state.error = null;
            })
            .addCase(fetchRoles.fulfilled, (state, action) => {
                state.loading = false;
                state.roles = action.payload.data;
                state.pageIndex = action.payload.pageIndex;
                state.pageSize = action.payload.pageSize;
                state.totalCount = action.payload.count;
            })
            .addCase(fetchRoles.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || "Error loading roles";
            })

  
            .addCase(deleteRole.pending, (state) => {
                state.error = null;
            })
            .addCase(deleteRole.fulfilled, (state, action) => {
                if (!action.payload) return;

                state.roles = state.roles.filter(r => r.id !== action.payload);
                state.totalCount = Math.max(0, state.totalCount - 1);
            })
            .addCase(deleteRole.rejected, (state, action) => {
                state.error =
                    action.error.message || "Error deleting role";
            })

            .addCase(updateRole.fulfilled, (state, action) => {
                const updated = action.payload;

                const index = state.roles.findIndex(
                    (r) => r.id === updated.id
                );

                if (index !== -1) {
                    state.roles[index] = {
                        ...state.roles[index],
                        ...updated,
                    };
                }
            })
            .addCase(updateRole.rejected, (state, action) => {
                state.error =
                    action.error.message || "Error updating role";
            })
            .addCase(createRole.fulfilled, (state, action) => {
                const newRole = action.payload;

                state.roles.unshift(newRole);

                state.totalCount += 1;
            })
            .addCase(createRole.rejected, (state, action) => {
                state.error =
                    action.error.message || "Error creating role";
            })
    },
});

export default roleSlice.reducer;