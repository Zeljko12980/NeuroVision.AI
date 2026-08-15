import {
    createAsyncThunk,
    createSlice
} from "@reduxjs/toolkit";


import {
    GovernmentHistoryRequest,
    GovernmentHistoryResponse,
    GovernmentHistoryKey,
    CreateGovernmentHistoryResponse,
    PaginatedGovernmentHistoryResponse
} from "./governmentHistory.types";


import {
    getGovernmentHistories,
    getGovernmentHistoryByKey,
    createGovernmentHistory,
    updateGovernmentHistory,
    deleteGovernmentHistory,
} from "./governmentHistory.service";





interface GovernmentHistoryState {

    items: GovernmentHistoryResponse[];

    selected: GovernmentHistoryResponse | null;

    totalCount: number;

    loading: boolean;

    error: string | null;

}




const initialState: GovernmentHistoryState = {

    items: [],

    selected: null,

    totalCount: 0,

    loading: false,

    error: null,

};





export const fetchGovernmentHistories = createAsyncThunk<
    PaginatedGovernmentHistoryResponse,
    {
        pageIndex: number;
        pageSize: number;
        search?: string;
    },
    {
        rejectValue: string;
    }
>(

    "governmentHistories/fetchAll",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await getGovernmentHistories(
                request.pageIndex,
                request.pageSize,
                request.search
            );

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to fetch governmentHistories"
            );

        }

    }

);


export const fetchGovernmentHistoryByKey = createAsyncThunk<
    GovernmentHistoryResponse,
    GovernmentHistoryKey,
    {
        rejectValue: string;
    }
>(

    "governmentHistories/getByKey",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            return await getGovernmentHistoryByKey(key);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to get governmentHistory"
            );

        }

    }

);


export const createNewGovernmentHistory = createAsyncThunk<
    CreateGovernmentHistoryResponse,
    GovernmentHistoryRequest,
    {
        rejectValue: string;
    }
>(

    "governmentHistories/create",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await createGovernmentHistory(request);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to create governmentHistory"
            );

        }

    }

);


export const updateExistingGovernmentHistory = createAsyncThunk<
    GovernmentHistoryKey,
    {
        key: GovernmentHistoryKey;
        request: GovernmentHistoryRequest;
    },
    {
        rejectValue: string;
    }
>(

    "governmentHistories/update",

    async (
        { key, request },

        { rejectWithValue }

    ) => {

        try {


            await updateGovernmentHistory(
                key,
                request
            );


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to update governmentHistory"
            );

        }

    }

);


export const deleteExistingGovernmentHistory = createAsyncThunk<
    GovernmentHistoryKey,
    GovernmentHistoryKey,
    {
        rejectValue: string;
    }
>(

    "governmentHistories/delete",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            await deleteGovernmentHistory(key);


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to delete governmentHistory"
            );

        }

    }

);


const governmentHistorySlice = createSlice({

    name: "governmentHistories",

    initialState,


    reducers: {


        clearSelected(state) {

            state.selected = null;

        },


        clearError(state) {

            state.error = null;

        }


    },



    extraReducers: builder => {


        builder
            .addCase(
                fetchGovernmentHistories.pending,
                state => {

                    state.loading = true;
                    state.error = null;

                }
            )
            .addCase(
                fetchGovernmentHistories.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items =
                        action.payload.data;


                    state.totalCount =
                        action.payload.count;

                }
            )
            .addCase(
                fetchGovernmentHistories.rejected,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.error =
                        action.payload ?? null;

                }
            )
            .addCase(
                fetchGovernmentHistoryByKey.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                fetchGovernmentHistoryByKey.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.selected =
                        action.payload;

                }
            )
            .addCase(
                fetchGovernmentHistoryByKey.rejected,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.error =
                        action.payload ?? null;

                }
            )
            .addCase(
                createNewGovernmentHistory.pending,
                state => {

                    state.loading = true;

                    state.error = null;

                }
            )
            .addCase(
                createNewGovernmentHistory.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items.push(
                        action.payload
                    );

                }
            )
            .addCase(
                createNewGovernmentHistory.rejected,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.error =
                        action.payload ?? null;

                }
            )
            .addCase(
                updateExistingGovernmentHistory.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                updateExistingGovernmentHistory.fulfilled,
                state => {

                    state.loading = false;

                }
            )
            .addCase(
                updateExistingGovernmentHistory.rejected,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.error =
                        action.payload ?? null;

                }
            )
            .addCase(
                deleteExistingGovernmentHistory.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                deleteExistingGovernmentHistory.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items =
                        state.items.filter(
                            x =>
                                !(x.countryCode === action.payload.countryCode && x.sequenceNumber === action.payload.sequenceNumber)
                        );


                    state.totalCount--;

                }
            )
            .addCase(
                deleteExistingGovernmentHistory.rejected,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.error =
                        action.payload ?? null;

                }
            );


    }


});

export const selectGovernmentHistories = (
    state: {
        governmentHistories: GovernmentHistoryState
    }
) => state.governmentHistories.items;

export const {
    clearSelected,
    clearError

} = governmentHistorySlice.actions;



export default governmentHistorySlice.reducer;
