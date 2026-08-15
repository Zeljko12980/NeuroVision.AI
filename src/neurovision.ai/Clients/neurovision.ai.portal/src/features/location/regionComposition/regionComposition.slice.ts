import {
    createAsyncThunk,
    createSlice
} from "@reduxjs/toolkit";


import {
    RegionCompositionRequest,
    RegionCompositionResponse,
    RegionCompositionKey,
    CreateRegionCompositionResponse,
    PaginatedRegionCompositionResponse
} from "./regionComposition.types";


import {
    getRegionCompositions,
    getRegionCompositionByKey,
    createRegionComposition,
    updateRegionComposition,
    deleteRegionComposition,
} from "./regionComposition.service";





interface RegionCompositionState {

    items: RegionCompositionResponse[];

    selected: RegionCompositionResponse | null;

    totalCount: number;

    loading: boolean;

    error: string | null;

}




const initialState: RegionCompositionState = {

    items: [],

    selected: null,

    totalCount: 0,

    loading: false,

    error: null,

};





export const fetchRegionCompositions = createAsyncThunk<
    PaginatedRegionCompositionResponse,
    {
        pageIndex: number;
        pageSize: number;
        search?: string;
    },
    {
        rejectValue: string;
    }
>(

    "regionCompositions/fetchAll",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await getRegionCompositions(
                request.pageIndex,
                request.pageSize,
                request.search
            );

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to fetch regionCompositions"
            );

        }

    }

);


export const fetchRegionCompositionByKey = createAsyncThunk<
    RegionCompositionResponse,
    RegionCompositionKey,
    {
        rejectValue: string;
    }
>(

    "regionCompositions/getByKey",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            return await getRegionCompositionByKey(key);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to get regionComposition"
            );

        }

    }

);


export const createNewRegionComposition = createAsyncThunk<
    CreateRegionCompositionResponse,
    RegionCompositionRequest,
    {
        rejectValue: string;
    }
>(

    "regionCompositions/create",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await createRegionComposition(request);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to create regionComposition"
            );

        }

    }

);


export const updateExistingRegionComposition = createAsyncThunk<
    RegionCompositionKey,
    {
        key: RegionCompositionKey;
        request: RegionCompositionRequest;
    },
    {
        rejectValue: string;
    }
>(

    "regionCompositions/update",

    async (
        { key, request },

        { rejectWithValue }

    ) => {

        try {


            await updateRegionComposition(
                key,
                request
            );


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to update regionComposition"
            );

        }

    }

);


export const deleteExistingRegionComposition = createAsyncThunk<
    RegionCompositionKey,
    RegionCompositionKey,
    {
        rejectValue: string;
    }
>(

    "regionCompositions/delete",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            await deleteRegionComposition(key);


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to delete regionComposition"
            );

        }

    }

);


const regionCompositionSlice = createSlice({

    name: "regionCompositions",

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
                fetchRegionCompositions.pending,
                state => {

                    state.loading = true;
                    state.error = null;

                }
            )
            .addCase(
                fetchRegionCompositions.fulfilled,
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
                fetchRegionCompositions.rejected,
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
                fetchRegionCompositionByKey.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                fetchRegionCompositionByKey.fulfilled,
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
                fetchRegionCompositionByKey.rejected,
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
                createNewRegionComposition.pending,
                state => {

                    state.loading = true;

                    state.error = null;

                }
            )
            .addCase(
                createNewRegionComposition.fulfilled,
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
                createNewRegionComposition.rejected,
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
                updateExistingRegionComposition.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                updateExistingRegionComposition.fulfilled,
                state => {

                    state.loading = false;

                }
            )
            .addCase(
                updateExistingRegionComposition.rejected,
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
                deleteExistingRegionComposition.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                deleteExistingRegionComposition.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items =
                        state.items.filter(
                            x =>
                                !(x.parentRegionTypeCode === action.payload.parentRegionTypeCode && x.parentRegionCode === action.payload.parentRegionCode && x.memberRegionTypeCode === action.payload.memberRegionTypeCode && x.memberRegionCode === action.payload.memberRegionCode)
                        );


                    state.totalCount--;

                }
            )
            .addCase(
                deleteExistingRegionComposition.rejected,
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

export const selectRegionCompositions = (
    state: {
        regionCompositions: RegionCompositionState
    }
) => state.regionCompositions.items;

export const {
    clearSelected,
    clearError

} = regionCompositionSlice.actions;



export default regionCompositionSlice.reducer;
