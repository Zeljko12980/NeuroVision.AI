import {
    createAsyncThunk,
    createSlice
} from "@reduxjs/toolkit";


import {
    RegionRequest,
    RegionResponse,
    RegionKey,
    CreateRegionResponse,
    PaginatedRegionResponse
} from "./region.types";


import {
    getRegions,
    getRegionByKey,
    createRegion,
    updateRegion,
    deleteRegion,
} from "./region.service";





interface RegionState {

    items: RegionResponse[];

    selected: RegionResponse | null;

    totalCount: number;

    loading: boolean;

    error: string | null;

}




const initialState: RegionState = {

    items: [],

    selected: null,

    totalCount: 0,

    loading: false,

    error: null,

};





export const fetchRegions = createAsyncThunk<
    PaginatedRegionResponse,
    {
        pageIndex: number;
        pageSize: number;
        search?: string;
    },
    {
        rejectValue: string;
    }
>(

    "regions/fetchAll",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await getRegions(
                request.pageIndex,
                request.pageSize,
                request.search
            );

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to fetch regions"
            );

        }

    }

);


export const fetchRegionByKey = createAsyncThunk<
    RegionResponse,
    RegionKey,
    {
        rejectValue: string;
    }
>(

    "regions/getByKey",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            return await getRegionByKey(key);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to get region"
            );

        }

    }

);


export const createNewRegion = createAsyncThunk<
    CreateRegionResponse,
    RegionRequest,
    {
        rejectValue: string;
    }
>(

    "regions/create",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await createRegion(request);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to create region"
            );

        }

    }

);


export const updateExistingRegion = createAsyncThunk<
    RegionKey,
    {
        key: RegionKey;
        request: RegionRequest;
    },
    {
        rejectValue: string;
    }
>(

    "regions/update",

    async (
        { key, request },

        { rejectWithValue }

    ) => {

        try {


            await updateRegion(
                key,
                request
            );


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to update region"
            );

        }

    }

);


export const deleteExistingRegion = createAsyncThunk<
    RegionKey,
    RegionKey,
    {
        rejectValue: string;
    }
>(

    "regions/delete",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            await deleteRegion(key);


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to delete region"
            );

        }

    }

);


const regionSlice = createSlice({

    name: "regions",

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
                fetchRegions.pending,
                state => {

                    state.loading = true;
                    state.error = null;

                }
            )
            .addCase(
                fetchRegions.fulfilled,
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
                fetchRegions.rejected,
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
                fetchRegionByKey.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                fetchRegionByKey.fulfilled,
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
                fetchRegionByKey.rejected,
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
                createNewRegion.pending,
                state => {

                    state.loading = true;

                    state.error = null;

                }
            )
            .addCase(
                createNewRegion.fulfilled,
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
                createNewRegion.rejected,
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
                updateExistingRegion.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                updateExistingRegion.fulfilled,
                state => {

                    state.loading = false;

                }
            )
            .addCase(
                updateExistingRegion.rejected,
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
                deleteExistingRegion.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                deleteExistingRegion.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items =
                        state.items.filter(
                            x =>
                                !(x.typeCode === action.payload.typeCode && x.code === action.payload.code)
                        );


                    state.totalCount--;

                }
            )
            .addCase(
                deleteExistingRegion.rejected,
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

export const selectRegions = (
    state: {
        regions: RegionState
    }
) => state.regions.items;

export const {
    clearSelected,
    clearError

} = regionSlice.actions;



export default regionSlice.reducer;
