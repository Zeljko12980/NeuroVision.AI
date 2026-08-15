import {
    createAsyncThunk,
    createSlice
} from "@reduxjs/toolkit";


import {
    CountryRequest,
    CountryResponse,
    CreateCountryResponse,
    PaginatedCountryResponse
} from "./country.types";


import {
    getCountries,
    getCountryByCode,
    createCountry,
    updateCountry,
    deleteCountry,
} from "./country.service";





interface CountryState {

    items: CountryResponse[];

    selected: CountryResponse | null;

    totalCount: number;

    loading: boolean;

    error: string | null;

}




const initialState: CountryState = {

    items: [],

    selected: null,

    totalCount: 0,

    loading: false,

    error: null,

};






export const fetchCountries = createAsyncThunk<
    PaginatedCountryResponse,
    {
        pageIndex: number;
        pageSize: number;
        search?: string;
    },
    {
        rejectValue: string;
    }
>(

    "countries/fetchAll",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await getCountries(
                request.pageIndex,
                request.pageSize,
                request.search
            );

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to fetch countries"
            );

        }

    }

);


export const fetchCountryByCode = createAsyncThunk<
    CountryResponse,
    string,
    {
        rejectValue: string;
    }
>(

    "countries/getByCode",

    async (
        code,
        { rejectWithValue }
    ) => {

        try {

            return await getCountryByCode(code);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to get country"
            );

        }

    }

);


export const createNewCountry = createAsyncThunk<
    CreateCountryResponse,
    CountryRequest,
    {
        rejectValue: string;
    }
>(

    "countries/create",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await createCountry(request);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to create country"
            );

        }

    }

);


export const updateExistingCountry = createAsyncThunk<
    string,
    {
        code: string;
        request: CountryRequest;
    },
    {
        rejectValue: string;
    }
>(

    "countries/update",

    async (
        {
            code,
            request
        },

        { rejectWithValue }

    ) => {

        try {


            await updateCountry(
                code,
                request
            );


            return code;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to update country"
            );

        }

    }

);


export const deleteExistingCountry = createAsyncThunk<
    string,
    string,
    {
        rejectValue: string;
    }
>(

    "countries/delete",

    async (
        code,
        { rejectWithValue }
    ) => {

        try {

            await deleteCountry(code);


            return code;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to delete country"
            );

        }

    }

);


const countrySlice = createSlice({

    name: "countries",

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
                fetchCountries.pending,
                state => {

                    state.loading = true;
                    state.error = null;

                }
            )
            .addCase(
                fetchCountries.fulfilled,
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
                fetchCountries.rejected,
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
                fetchCountryByCode.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                fetchCountryByCode.fulfilled,
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
                fetchCountryByCode.rejected,
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
                createNewCountry.pending,
                state => {

                    state.loading = true;

                    state.error = null;

                }
            )
            .addCase(
                createNewCountry.fulfilled,
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
                createNewCountry.rejected,
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
                updateExistingCountry.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                updateExistingCountry.fulfilled,
                state => {

                    state.loading = false;

                }
            )
            .addCase(
                updateExistingCountry.rejected,
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
                deleteExistingCountry.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                deleteExistingCountry.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items =
                        state.items.filter(
                            x =>
                                x.code !== action.payload
                        );


                    state.totalCount--;

                }
            )
            .addCase(
                deleteExistingCountry.rejected,
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

export const selectCountries = (
    state: {
        countries: CountryState
    }
) => state.countries.items;

export const {
    clearSelected,
    clearError

} = countrySlice.actions;



export default countrySlice.reducer;