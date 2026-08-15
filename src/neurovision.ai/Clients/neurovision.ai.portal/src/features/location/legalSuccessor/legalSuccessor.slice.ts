import {
    createAsyncThunk,
    createSlice
} from "@reduxjs/toolkit";


import {
    LegalSuccessorRequest,
    LegalSuccessorResponse,
    LegalSuccessorKey,
    CreateLegalSuccessorResponse,
    PaginatedLegalSuccessorResponse
} from "./legalSuccessor.types";


import {
    getLegalSuccessors,
    getLegalSuccessorByKey,
    createLegalSuccessor,
    updateLegalSuccessor,
    deleteLegalSuccessor,
} from "./legalSuccessor.service";





interface LegalSuccessorState {

    items: LegalSuccessorResponse[];

    selected: LegalSuccessorResponse | null;

    totalCount: number;

    loading: boolean;

    error: string | null;

}




const initialState: LegalSuccessorState = {

    items: [],

    selected: null,

    totalCount: 0,

    loading: false,

    error: null,

};





export const fetchLegalSuccessors = createAsyncThunk<
    PaginatedLegalSuccessorResponse,
    {
        pageIndex: number;
        pageSize: number;
        search?: string;
    },
    {
        rejectValue: string;
    }
>(

    "legalSuccessors/fetchAll",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await getLegalSuccessors(
                request.pageIndex,
                request.pageSize,
                request.search
            );

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to fetch legalSuccessors"
            );

        }

    }

);


export const fetchLegalSuccessorByKey = createAsyncThunk<
    LegalSuccessorResponse,
    LegalSuccessorKey,
    {
        rejectValue: string;
    }
>(

    "legalSuccessors/getByKey",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            return await getLegalSuccessorByKey(key);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to get legalSuccessor"
            );

        }

    }

);


export const createNewLegalSuccessor = createAsyncThunk<
    CreateLegalSuccessorResponse,
    LegalSuccessorRequest,
    {
        rejectValue: string;
    }
>(

    "legalSuccessors/create",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await createLegalSuccessor(request);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to create legalSuccessor"
            );

        }

    }

);


export const updateExistingLegalSuccessor = createAsyncThunk<
    LegalSuccessorKey,
    {
        key: LegalSuccessorKey;
        request: LegalSuccessorRequest;
    },
    {
        rejectValue: string;
    }
>(

    "legalSuccessors/update",

    async (
        { key, request },

        { rejectWithValue }

    ) => {

        try {


            await updateLegalSuccessor(
                key,
                request
            );


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to update legalSuccessor"
            );

        }

    }

);


export const deleteExistingLegalSuccessor = createAsyncThunk<
    LegalSuccessorKey,
    LegalSuccessorKey,
    {
        rejectValue: string;
    }
>(

    "legalSuccessors/delete",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            await deleteLegalSuccessor(key);


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to delete legalSuccessor"
            );

        }

    }

);


const legalSuccessorSlice = createSlice({

    name: "legalSuccessors",

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
                fetchLegalSuccessors.pending,
                state => {

                    state.loading = true;
                    state.error = null;

                }
            )
            .addCase(
                fetchLegalSuccessors.fulfilled,
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
                fetchLegalSuccessors.rejected,
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
                fetchLegalSuccessorByKey.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                fetchLegalSuccessorByKey.fulfilled,
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
                fetchLegalSuccessorByKey.rejected,
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
                createNewLegalSuccessor.pending,
                state => {

                    state.loading = true;

                    state.error = null;

                }
            )
            .addCase(
                createNewLegalSuccessor.fulfilled,
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
                createNewLegalSuccessor.rejected,
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
                updateExistingLegalSuccessor.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                updateExistingLegalSuccessor.fulfilled,
                state => {

                    state.loading = false;

                }
            )
            .addCase(
                updateExistingLegalSuccessor.rejected,
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
                deleteExistingLegalSuccessor.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                deleteExistingLegalSuccessor.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items =
                        state.items.filter(
                            x =>
                                !(x.successorCountryCode === action.payload.successorCountryCode && x.predecessorCountryCode === action.payload.predecessorCountryCode)
                        );


                    state.totalCount--;

                }
            )
            .addCase(
                deleteExistingLegalSuccessor.rejected,
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

export const selectLegalSuccessors = (
    state: {
        legalSuccessors: LegalSuccessorState
    }
) => state.legalSuccessors.items;

export const {
    clearSelected,
    clearError

} = legalSuccessorSlice.actions;



export default legalSuccessorSlice.reducer;
