import {
    createAsyncThunk,
    createSlice
} from "@reduxjs/toolkit";


import {
    MunicipalitySettlementCoverageRequest,
    MunicipalitySettlementCoverageResponse,
    MunicipalitySettlementCoverageKey,
    CreateMunicipalitySettlementCoverageResponse,
    PaginatedMunicipalitySettlementCoverageResponse
} from "./municipalitySettlementCoverage.types";


import {
    getMunicipalitySettlementCoverages,
    getMunicipalitySettlementCoverageByKey,
    createMunicipalitySettlementCoverage,
    updateMunicipalitySettlementCoverage,
    deleteMunicipalitySettlementCoverage,
} from "./municipalitySettlementCoverage.service";





interface MunicipalitySettlementCoverageState {

    items: MunicipalitySettlementCoverageResponse[];

    selected: MunicipalitySettlementCoverageResponse | null;

    totalCount: number;

    loading: boolean;

    error: string | null;

}




const initialState: MunicipalitySettlementCoverageState = {

    items: [],

    selected: null,

    totalCount: 0,

    loading: false,

    error: null,

};





export const fetchMunicipalitySettlementCoverages = createAsyncThunk<
    PaginatedMunicipalitySettlementCoverageResponse,
    {
        pageIndex: number;
        pageSize: number;
        search?: string;
    },
    {
        rejectValue: string;
    }
>(

    "municipalitySettlementCoverages/fetchAll",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await getMunicipalitySettlementCoverages(
                request.pageIndex,
                request.pageSize,
                request.search
            );

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to fetch municipalitySettlementCoverages"
            );

        }

    }

);


export const fetchMunicipalitySettlementCoverageByKey = createAsyncThunk<
    MunicipalitySettlementCoverageResponse,
    MunicipalitySettlementCoverageKey,
    {
        rejectValue: string;
    }
>(

    "municipalitySettlementCoverages/getByKey",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            return await getMunicipalitySettlementCoverageByKey(key);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to get municipalitySettlementCoverage"
            );

        }

    }

);


export const createNewMunicipalitySettlementCoverage = createAsyncThunk<
    CreateMunicipalitySettlementCoverageResponse,
    MunicipalitySettlementCoverageRequest,
    {
        rejectValue: string;
    }
>(

    "municipalitySettlementCoverages/create",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await createMunicipalitySettlementCoverage(request);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to create municipalitySettlementCoverage"
            );

        }

    }

);


export const updateExistingMunicipalitySettlementCoverage = createAsyncThunk<
    MunicipalitySettlementCoverageKey,
    {
        key: MunicipalitySettlementCoverageKey;
        request: MunicipalitySettlementCoverageRequest;
    },
    {
        rejectValue: string;
    }
>(

    "municipalitySettlementCoverages/update",

    async (
        { key, request },

        { rejectWithValue }

    ) => {

        try {


            await updateMunicipalitySettlementCoverage(
                key,
                request
            );


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to update municipalitySettlementCoverage"
            );

        }

    }

);


export const deleteExistingMunicipalitySettlementCoverage = createAsyncThunk<
    MunicipalitySettlementCoverageKey,
    MunicipalitySettlementCoverageKey,
    {
        rejectValue: string;
    }
>(

    "municipalitySettlementCoverages/delete",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            await deleteMunicipalitySettlementCoverage(key);


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to delete municipalitySettlementCoverage"
            );

        }

    }

);


const municipalitySettlementCoverageSlice = createSlice({

    name: "municipalitySettlementCoverages",

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
                fetchMunicipalitySettlementCoverages.pending,
                state => {

                    state.loading = true;
                    state.error = null;

                }
            )
            .addCase(
                fetchMunicipalitySettlementCoverages.fulfilled,
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
                fetchMunicipalitySettlementCoverages.rejected,
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
                fetchMunicipalitySettlementCoverageByKey.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                fetchMunicipalitySettlementCoverageByKey.fulfilled,
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
                fetchMunicipalitySettlementCoverageByKey.rejected,
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
                createNewMunicipalitySettlementCoverage.pending,
                state => {

                    state.loading = true;

                    state.error = null;

                }
            )
            .addCase(
                createNewMunicipalitySettlementCoverage.fulfilled,
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
                createNewMunicipalitySettlementCoverage.rejected,
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
                updateExistingMunicipalitySettlementCoverage.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                updateExistingMunicipalitySettlementCoverage.fulfilled,
                state => {

                    state.loading = false;

                }
            )
            .addCase(
                updateExistingMunicipalitySettlementCoverage.rejected,
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
                deleteExistingMunicipalitySettlementCoverage.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                deleteExistingMunicipalitySettlementCoverage.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items =
                        state.items.filter(
                            x =>
                                !(x.countryCode === action.payload.countryCode && x.municipalityCode === action.payload.municipalityCode && x.settlementCode === action.payload.settlementCode)
                        );


                    state.totalCount--;

                }
            )
            .addCase(
                deleteExistingMunicipalitySettlementCoverage.rejected,
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

export const selectMunicipalitySettlementCoverages = (
    state: {
        municipalitySettlementCoverages: MunicipalitySettlementCoverageState
    }
) => state.municipalitySettlementCoverages.items;

export const {
    clearSelected,
    clearError

} = municipalitySettlementCoverageSlice.actions;



export default municipalitySettlementCoverageSlice.reducer;
