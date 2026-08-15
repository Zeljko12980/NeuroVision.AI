import {
    createAsyncThunk,
    createSlice
} from "@reduxjs/toolkit";


import {
    LocalCommunityRequest,
    LocalCommunityResponse,
    LocalCommunityKey,
    CreateLocalCommunityResponse,
    PaginatedLocalCommunityResponse
} from "./localCommunity.types";


import {
    getLocalCommunities,
    getLocalCommunityByKey,
    createLocalCommunity,
    updateLocalCommunity,
    deleteLocalCommunity,
} from "./localCommunity.service";





interface LocalCommunityState {

    items: LocalCommunityResponse[];

    selected: LocalCommunityResponse | null;

    totalCount: number;

    loading: boolean;

    error: string | null;

}




const initialState: LocalCommunityState = {

    items: [],

    selected: null,

    totalCount: 0,

    loading: false,

    error: null,

};





export const fetchLocalCommunities = createAsyncThunk<
    PaginatedLocalCommunityResponse,
    {
        pageIndex: number;
        pageSize: number;
        search?: string;
    },
    {
        rejectValue: string;
    }
>(

    "localCommunities/fetchAll",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await getLocalCommunities(
                request.pageIndex,
                request.pageSize,
                request.search
            );

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to fetch localCommunities"
            );

        }

    }

);


export const fetchLocalCommunityByKey = createAsyncThunk<
    LocalCommunityResponse,
    LocalCommunityKey,
    {
        rejectValue: string;
    }
>(

    "localCommunities/getByKey",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            return await getLocalCommunityByKey(key);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to get localCommunity"
            );

        }

    }

);


export const createNewLocalCommunity = createAsyncThunk<
    CreateLocalCommunityResponse,
    LocalCommunityRequest,
    {
        rejectValue: string;
    }
>(

    "localCommunities/create",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await createLocalCommunity(request);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to create localCommunity"
            );

        }

    }

);


export const updateExistingLocalCommunity = createAsyncThunk<
    LocalCommunityKey,
    {
        key: LocalCommunityKey;
        request: LocalCommunityRequest;
    },
    {
        rejectValue: string;
    }
>(

    "localCommunities/update",

    async (
        { key, request },

        { rejectWithValue }

    ) => {

        try {


            await updateLocalCommunity(
                key,
                request
            );


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to update localCommunity"
            );

        }

    }

);


export const deleteExistingLocalCommunity = createAsyncThunk<
    LocalCommunityKey,
    LocalCommunityKey,
    {
        rejectValue: string;
    }
>(

    "localCommunities/delete",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            await deleteLocalCommunity(key);


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to delete localCommunity"
            );

        }

    }

);


const localCommunitySlice = createSlice({

    name: "localCommunities",

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
                fetchLocalCommunities.pending,
                state => {

                    state.loading = true;
                    state.error = null;

                }
            )
            .addCase(
                fetchLocalCommunities.fulfilled,
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
                fetchLocalCommunities.rejected,
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
                fetchLocalCommunityByKey.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                fetchLocalCommunityByKey.fulfilled,
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
                fetchLocalCommunityByKey.rejected,
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
                createNewLocalCommunity.pending,
                state => {

                    state.loading = true;

                    state.error = null;

                }
            )
            .addCase(
                createNewLocalCommunity.fulfilled,
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
                createNewLocalCommunity.rejected,
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
                updateExistingLocalCommunity.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                updateExistingLocalCommunity.fulfilled,
                state => {

                    state.loading = false;

                }
            )
            .addCase(
                updateExistingLocalCommunity.rejected,
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
                deleteExistingLocalCommunity.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                deleteExistingLocalCommunity.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items =
                        state.items.filter(
                            x =>
                                !(x.countryCode === action.payload.countryCode && x.municipalityCode === action.payload.municipalityCode && x.identifier === action.payload.identifier)
                        );


                    state.totalCount--;

                }
            )
            .addCase(
                deleteExistingLocalCommunity.rejected,
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

export const selectLocalCommunities = (
    state: {
        localCommunities: LocalCommunityState
    }
) => state.localCommunities.items;

export const {
    clearSelected,
    clearError

} = localCommunitySlice.actions;



export default localCommunitySlice.reducer;
