import {
    createAsyncThunk,
    createSlice
} from "@reduxjs/toolkit";


import {
    CountryCompositionRequest,
    CountryCompositionResponse,
    CountryCompositionKey,
    CreateCountryCompositionResponse,
    PaginatedCountryCompositionResponse
} from "./countryComposition.types";


import {
    getCountryCompositions,
    getCountryCompositionByKey,
    createCountryComposition,
    updateCountryComposition,
    deleteCountryComposition,
} from "./countryComposition.service";





interface CountryCompositionState {

    items: CountryCompositionResponse[];

    selected: CountryCompositionResponse | null;

    totalCount: number;

    loading: boolean;

    error: string | null;

}




const initialState: CountryCompositionState = {

    items: [],

    selected: null,

    totalCount: 0,

    loading: false,

    error: null,

};





export const fetchCountryCompositions = createAsyncThunk<
    PaginatedCountryCompositionResponse,
    {
        pageIndex: number;
        pageSize: number;
        search?: string;
    },
    {
        rejectValue: string;
    }
>(

    "countryCompositions/fetchAll",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await getCountryCompositions(
                request.pageIndex,
                request.pageSize,
                request.search
            );

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to fetch countryCompositions"
            );

        }

    }

);


export const fetchCountryCompositionByKey = createAsyncThunk<
    CountryCompositionResponse,
    CountryCompositionKey,
    {
        rejectValue: string;
    }
>(

    "countryCompositions/getByKey",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            return await getCountryCompositionByKey(key);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to get countryComposition"
            );

        }

    }

);


export const createNewCountryComposition = createAsyncThunk<
    CreateCountryCompositionResponse,
    CountryCompositionRequest,
    {
        rejectValue: string;
    }
>(

    "countryCompositions/create",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await createCountryComposition(request);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to create countryComposition"
            );

        }

    }

);


export const updateExistingCountryComposition = createAsyncThunk<
    CountryCompositionKey,
    {
        key: CountryCompositionKey;
        request: CountryCompositionRequest;
    },
    {
        rejectValue: string;
    }
>(

    "countryCompositions/update",

    async (
        { key, request },

        { rejectWithValue }

    ) => {

        try {


            await updateCountryComposition(
                key,
                request
            );


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to update countryComposition"
            );

        }

    }

);


export const deleteExistingCountryComposition = createAsyncThunk<
    CountryCompositionKey,
    CountryCompositionKey,
    {
        rejectValue: string;
    }
>(

    "countryCompositions/delete",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            await deleteCountryComposition(key);


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to delete countryComposition"
            );

        }

    }

);


const countryCompositionSlice = createSlice({

    name: "countryCompositions",

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
                fetchCountryCompositions.pending,
                state => {

                    state.loading = true;
                    state.error = null;

                }
            )
            .addCase(
                fetchCountryCompositions.fulfilled,
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
                fetchCountryCompositions.rejected,
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
                fetchCountryCompositionByKey.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                fetchCountryCompositionByKey.fulfilled,
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
                fetchCountryCompositionByKey.rejected,
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
                createNewCountryComposition.pending,
                state => {

                    state.loading = true;

                    state.error = null;

                }
            )
            .addCase(
                createNewCountryComposition.fulfilled,
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
                createNewCountryComposition.rejected,
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
                updateExistingCountryComposition.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                updateExistingCountryComposition.fulfilled,
                state => {

                    state.loading = false;

                }
            )
            .addCase(
                updateExistingCountryComposition.rejected,
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
                deleteExistingCountryComposition.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                deleteExistingCountryComposition.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items =
                        state.items.filter(
                            x =>
                                !(x.unionCountryCode === action.payload.unionCountryCode && x.memberCountryCode === action.payload.memberCountryCode && x.sequenceNumber === action.payload.sequenceNumber)
                        );


                    state.totalCount--;

                }
            )
            .addCase(
                deleteExistingCountryComposition.rejected,
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

export const selectCountryCompositions = (
    state: {
        countryCompositions: CountryCompositionState
    }
) => state.countryCompositions.items;

export const {
    clearSelected,
    clearError

} = countryCompositionSlice.actions;



export default countryCompositionSlice.reducer;
