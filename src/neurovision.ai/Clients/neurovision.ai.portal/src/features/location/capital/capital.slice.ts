import {
    createAsyncThunk,
    createSlice
} from "@reduxjs/toolkit";


import {
    CapitalRequest,
    CapitalResponse,
    CreateCapitalResponse,
    PaginatedCapitalResponse
} from "./capital.types";


import {
    getCapitals,
    getCapitalByCountryCode,
    createCapital,
    updateCapital,
    deleteCapital,
} from "./capital.service";





interface CapitalState {

    items: CapitalResponse[];

    selected: CapitalResponse | null;

    totalCount: number;

    loading: boolean;

    error: string | null;

}




const initialState: CapitalState = {

    items: [],

    selected: null,

    totalCount: 0,

    loading: false,

    error: null,

};





export const fetchCapitals = createAsyncThunk<
    PaginatedCapitalResponse,
    {
        pageIndex: number;
        pageSize: number;
        search?: string;
    },
    {
        rejectValue: string;
    }
>(

    "capitals/fetchAll",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await getCapitals(
                request.pageIndex,
                request.pageSize,
                request.search
            );

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to fetch capitals"
            );

        }

    }

);


export const fetchCapitalByKey = createAsyncThunk<
    CapitalResponse,
    string,
    {
        rejectValue: string;
    }
>(

    "capitals/getByKey",

    async (
        countryCode,
        { rejectWithValue }
    ) => {

        try {

            return await getCapitalByCountryCode(countryCode);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to get capital"
            );

        }

    }

);


export const createNewCapital = createAsyncThunk<
    CreateCapitalResponse,
    CapitalRequest,
    {
        rejectValue: string;
    }
>(

    "capitals/create",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await createCapital(request);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to create capital"
            );

        }

    }

);


export const updateExistingCapital = createAsyncThunk<
    string,
    {
        countryCode: string;
        request: CapitalRequest;
    },
    {
        rejectValue: string;
    }
>(

    "capitals/update",

    async (
        { countryCode, request },

        { rejectWithValue }

    ) => {

        try {


            await updateCapital(
                countryCode,
                request
            );


            return countryCode;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to update capital"
            );

        }

    }

);


export const deleteExistingCapital = createAsyncThunk<
    string,
    string,
    {
        rejectValue: string;
    }
>(

    "capitals/delete",

    async (
        countryCode,
        { rejectWithValue }
    ) => {

        try {

            await deleteCapital(countryCode);


            return countryCode;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to delete capital"
            );

        }

    }

);


const capitalSlice = createSlice({

    name: "capitals",

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
                fetchCapitals.pending,
                state => {

                    state.loading = true;
                    state.error = null;

                }
            )
            .addCase(
                fetchCapitals.fulfilled,
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
                fetchCapitals.rejected,
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
                fetchCapitalByKey.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                fetchCapitalByKey.fulfilled,
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
                fetchCapitalByKey.rejected,
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
                createNewCapital.pending,
                state => {

                    state.loading = true;

                    state.error = null;

                }
            )
            .addCase(
                createNewCapital.fulfilled,
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
                createNewCapital.rejected,
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
                updateExistingCapital.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                updateExistingCapital.fulfilled,
                state => {

                    state.loading = false;

                }
            )
            .addCase(
                updateExistingCapital.rejected,
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
                deleteExistingCapital.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                deleteExistingCapital.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items =
                        state.items.filter(
                            x =>
                                x.countryCode !== action.payload
                        );


                    state.totalCount--;

                }
            )
            .addCase(
                deleteExistingCapital.rejected,
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

export const selectCapitals = (
    state: {
        capitals: CapitalState
    }
) => state.capitals.items;

export const {
    clearSelected,
    clearError

} = capitalSlice.actions;



export default capitalSlice.reducer;
