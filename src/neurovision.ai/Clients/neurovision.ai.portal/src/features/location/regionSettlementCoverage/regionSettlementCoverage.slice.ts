import {
    createAsyncThunk,
    createSlice
} from "@reduxjs/toolkit";


import {
    RegionSettlementCoverageRequest,
    RegionSettlementCoverageResponse,
    RegionSettlementCoverageKey,
    CreateRegionSettlementCoverageResponse,
    PaginatedRegionSettlementCoverageResponse
} from "./regionSettlementCoverage.types";


import {
    getRegionSettlementCoverages,
    getRegionSettlementCoverageByKey,
    createRegionSettlementCoverage,
    updateRegionSettlementCoverage,
    deleteRegionSettlementCoverage,
} from "./regionSettlementCoverage.service";





interface RegionSettlementCoverageState {

    items: RegionSettlementCoverageResponse[];

    selected: RegionSettlementCoverageResponse | null;

    totalCount: number;

    loading: boolean;

    error: string | null;

}




const initialState: RegionSettlementCoverageState = {

    items: [],

    selected: null,

    totalCount: 0,

    loading: false,

    error: null,

};





export const fetchRegionSettlementCoverages = createAsyncThunk<
    PaginatedRegionSettlementCoverageResponse,
    {
        pageIndex: number;
        pageSize: number;
        search?: string;
    },
    {
        rejectValue: string;
    }
>(

    "regionSettlementCoverages/fetchAll",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await getRegionSettlementCoverages(
                request.pageIndex,
                request.pageSize,
                request.search
            );

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to fetch regionSettlementCoverages"
            );

        }

    }

);


export const fetchRegionSettlementCoverageByKey = createAsyncThunk<
    RegionSettlementCoverageResponse,
    RegionSettlementCoverageKey,
    {
        rejectValue: string;
    }
>(

    "regionSettlementCoverages/getByKey",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            return await getRegionSettlementCoverageByKey(key);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to get regionSettlementCoverage"
            );

        }

    }

);


export const createNewRegionSettlementCoverage = createAsyncThunk<
    CreateRegionSettlementCoverageResponse,
    RegionSettlementCoverageRequest,
    {
        rejectValue: string;
    }
>(

    "regionSettlementCoverages/create",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await createRegionSettlementCoverage(request);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to create regionSettlementCoverage"
            );

        }

    }

);


export const updateExistingRegionSettlementCoverage = createAsyncThunk<
    RegionSettlementCoverageKey,
    {
        key: RegionSettlementCoverageKey;
        request: RegionSettlementCoverageRequest;
    },
    {
        rejectValue: string;
    }
>(

    "regionSettlementCoverages/update",

    async (
        { key, request },

        { rejectWithValue }

    ) => {

        try {


            await updateRegionSettlementCoverage(
                key,
                request
            );


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to update regionSettlementCoverage"
            );

        }

    }

);


export const deleteExistingRegionSettlementCoverage = createAsyncThunk<
    RegionSettlementCoverageKey,
    RegionSettlementCoverageKey,
    {
        rejectValue: string;
    }
>(

    "regionSettlementCoverages/delete",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            await deleteRegionSettlementCoverage(key);


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to delete regionSettlementCoverage"
            );

        }

    }

);


const regionSettlementCoverageSlice = createSlice({

    name: "regionSettlementCoverages",

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
                fetchRegionSettlementCoverages.pending,
                state => {

                    state.loading = true;
                    state.error = null;

                }
            )
            .addCase(
                fetchRegionSettlementCoverages.fulfilled,
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
                fetchRegionSettlementCoverages.rejected,
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
                fetchRegionSettlementCoverageByKey.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                fetchRegionSettlementCoverageByKey.fulfilled,
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
                fetchRegionSettlementCoverageByKey.rejected,
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
                createNewRegionSettlementCoverage.pending,
                state => {

                    state.loading = true;

                    state.error = null;

                }
            )
            .addCase(
                createNewRegionSettlementCoverage.fulfilled,
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
                createNewRegionSettlementCoverage.rejected,
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
                updateExistingRegionSettlementCoverage.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                updateExistingRegionSettlementCoverage.fulfilled,
                state => {

                    state.loading = false;

                }
            )
            .addCase(
                updateExistingRegionSettlementCoverage.rejected,
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
                deleteExistingRegionSettlementCoverage.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                deleteExistingRegionSettlementCoverage.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items =
                        state.items.filter(
                            x =>
                                !(x.regionTypeCode === action.payload.regionTypeCode && x.regionCode === action.payload.regionCode && x.countryCode === action.payload.countryCode && x.settlementCode === action.payload.settlementCode)
                        );


                    state.totalCount--;

                }
            )
            .addCase(
                deleteExistingRegionSettlementCoverage.rejected,
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

export const selectRegionSettlementCoverages = (
    state: {
        regionSettlementCoverages: RegionSettlementCoverageState
    }
) => state.regionSettlementCoverages.items;

export const {
    clearSelected,
    clearError

} = regionSettlementCoverageSlice.actions;



export default regionSettlementCoverageSlice.reducer;
