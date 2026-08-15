import {
    createAsyncThunk,
    createSlice
} from "@reduxjs/toolkit";


import {
    LocalCommunityCoverageRequest,
    LocalCommunityCoverageResponse,
    LocalCommunityCoverageKey,
    CreateLocalCommunityCoverageResponse,
    PaginatedLocalCommunityCoverageResponse
} from "./localCommunityCoverage.types";


import {
    getLocalCommunityCoverages,
    getLocalCommunityCoverageByKey,
    createLocalCommunityCoverage,
    updateLocalCommunityCoverage,
    deleteLocalCommunityCoverage,
} from "./localCommunityCoverage.service";





interface LocalCommunityCoverageState {

    items: LocalCommunityCoverageResponse[];

    selected: LocalCommunityCoverageResponse | null;

    totalCount: number;

    loading: boolean;

    error: string | null;

}




const initialState: LocalCommunityCoverageState = {

    items: [],

    selected: null,

    totalCount: 0,

    loading: false,

    error: null,

};





export const fetchLocalCommunityCoverages = createAsyncThunk<
    PaginatedLocalCommunityCoverageResponse,
    {
        pageIndex: number;
        pageSize: number;
        search?: string;
    },
    {
        rejectValue: string;
    }
>(

    "localCommunityCoverages/fetchAll",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await getLocalCommunityCoverages(
                request.pageIndex,
                request.pageSize,
                request.search
            );

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to fetch localCommunityCoverages"
            );

        }

    }

);


export const fetchLocalCommunityCoverageByKey = createAsyncThunk<
    LocalCommunityCoverageResponse,
    LocalCommunityCoverageKey,
    {
        rejectValue: string;
    }
>(

    "localCommunityCoverages/getByKey",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            return await getLocalCommunityCoverageByKey(key);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to get localCommunityCoverage"
            );

        }

    }

);


export const createNewLocalCommunityCoverage = createAsyncThunk<
    CreateLocalCommunityCoverageResponse,
    LocalCommunityCoverageRequest,
    {
        rejectValue: string;
    }
>(

    "localCommunityCoverages/create",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await createLocalCommunityCoverage(request);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to create localCommunityCoverage"
            );

        }

    }

);


export const updateExistingLocalCommunityCoverage = createAsyncThunk<
    LocalCommunityCoverageKey,
    {
        key: LocalCommunityCoverageKey;
        request: LocalCommunityCoverageRequest;
    },
    {
        rejectValue: string;
    }
>(

    "localCommunityCoverages/update",

    async (
        { key, request },

        { rejectWithValue }

    ) => {

        try {


            await updateLocalCommunityCoverage(
                key,
                request
            );


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to update localCommunityCoverage"
            );

        }

    }

);


export const deleteExistingLocalCommunityCoverage = createAsyncThunk<
    LocalCommunityCoverageKey,
    LocalCommunityCoverageKey,
    {
        rejectValue: string;
    }
>(

    "localCommunityCoverages/delete",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            await deleteLocalCommunityCoverage(key);


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to delete localCommunityCoverage"
            );

        }

    }

);


const localCommunityCoverageSlice = createSlice({

    name: "localCommunityCoverages",

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
                fetchLocalCommunityCoverages.pending,
                state => {

                    state.loading = true;
                    state.error = null;

                }
            )
            .addCase(
                fetchLocalCommunityCoverages.fulfilled,
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
                fetchLocalCommunityCoverages.rejected,
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
                fetchLocalCommunityCoverageByKey.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                fetchLocalCommunityCoverageByKey.fulfilled,
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
                fetchLocalCommunityCoverageByKey.rejected,
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
                createNewLocalCommunityCoverage.pending,
                state => {

                    state.loading = true;

                    state.error = null;

                }
            )
            .addCase(
                createNewLocalCommunityCoverage.fulfilled,
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
                createNewLocalCommunityCoverage.rejected,
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
                updateExistingLocalCommunityCoverage.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                updateExistingLocalCommunityCoverage.fulfilled,
                state => {

                    state.loading = false;

                }
            )
            .addCase(
                updateExistingLocalCommunityCoverage.rejected,
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
                deleteExistingLocalCommunityCoverage.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                deleteExistingLocalCommunityCoverage.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items =
                        state.items.filter(
                            x =>
                                !(x.countryCode === action.payload.countryCode && x.municipalityCode === action.payload.municipalityCode && x.localCommunityIdentifier === action.payload.localCommunityIdentifier && x.settlementCode === action.payload.settlementCode)
                        );


                    state.totalCount--;

                }
            )
            .addCase(
                deleteExistingLocalCommunityCoverage.rejected,
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

export const selectLocalCommunityCoverages = (
    state: {
        localCommunityCoverages: LocalCommunityCoverageState
    }
) => state.localCommunityCoverages.items;

export const {
    clearSelected,
    clearError

} = localCommunityCoverageSlice.actions;



export default localCommunityCoverageSlice.reducer;
