import {
    createAsyncThunk,
    createSlice
} from "@reduxjs/toolkit";


import {
    MunicipalityRequest,
    MunicipalityResponse,
    MunicipalityKey,
    CreateMunicipalityResponse,
    PaginatedMunicipalityResponse
} from "./municipality.types";


import {
    getMunicipalities,
    getMunicipalityByKey,
    createMunicipality,
    updateMunicipality,
    deleteMunicipality,
} from "./municipality.service";





interface MunicipalityState {

    items: MunicipalityResponse[];

    selected: MunicipalityResponse | null;

    totalCount: number;

    loading: boolean;

    error: string | null;

}




const initialState: MunicipalityState = {

    items: [],

    selected: null,

    totalCount: 0,

    loading: false,

    error: null,

};





export const fetchMunicipalities = createAsyncThunk<
    PaginatedMunicipalityResponse,
    {
        pageIndex: number;
        pageSize: number;
        search?: string;
    },
    {
        rejectValue: string;
    }
>(

    "municipalities/fetchAll",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await getMunicipalities(
                request.pageIndex,
                request.pageSize,
                request.search
            );

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to fetch municipalities"
            );

        }

    }

);


export const fetchMunicipalityByKey = createAsyncThunk<
    MunicipalityResponse,
    MunicipalityKey,
    {
        rejectValue: string;
    }
>(

    "municipalities/getByKey",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            return await getMunicipalityByKey(key);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to get municipality"
            );

        }

    }

);


export const createNewMunicipality = createAsyncThunk<
    CreateMunicipalityResponse,
    MunicipalityRequest,
    {
        rejectValue: string;
    }
>(

    "municipalities/create",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await createMunicipality(request);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to create municipality"
            );

        }

    }

);


export const updateExistingMunicipality = createAsyncThunk<
    MunicipalityKey,
    {
        key: MunicipalityKey;
        request: MunicipalityRequest;
    },
    {
        rejectValue: string;
    }
>(

    "municipalities/update",

    async (
        { key, request },

        { rejectWithValue }

    ) => {

        try {


            await updateMunicipality(
                key,
                request
            );


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to update municipality"
            );

        }

    }

);


export const deleteExistingMunicipality = createAsyncThunk<
    MunicipalityKey,
    MunicipalityKey,
    {
        rejectValue: string;
    }
>(

    "municipalities/delete",

    async (
        key,
        { rejectWithValue }
    ) => {

        try {

            await deleteMunicipality(key);


            return key;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to delete municipality"
            );

        }

    }

);


const municipalitySlice = createSlice({

    name: "municipalities",

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
                fetchMunicipalities.pending,
                state => {

                    state.loading = true;
                    state.error = null;

                }
            )
            .addCase(
                fetchMunicipalities.fulfilled,
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
                fetchMunicipalities.rejected,
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
                fetchMunicipalityByKey.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                fetchMunicipalityByKey.fulfilled,
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
                fetchMunicipalityByKey.rejected,
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
                createNewMunicipality.pending,
                state => {

                    state.loading = true;

                    state.error = null;

                }
            )
            .addCase(
                createNewMunicipality.fulfilled,
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
                createNewMunicipality.rejected,
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
                updateExistingMunicipality.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                updateExistingMunicipality.fulfilled,
                state => {

                    state.loading = false;

                }
            )
            .addCase(
                updateExistingMunicipality.rejected,
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
                deleteExistingMunicipality.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                deleteExistingMunicipality.fulfilled,
                (
                    state,
                    action
                ) => {

                    state.loading = false;


                    state.items =
                        state.items.filter(
                            x =>
                                !(x.countryCode === action.payload.countryCode && x.code === action.payload.code)
                        );


                    state.totalCount--;

                }
            )
            .addCase(
                deleteExistingMunicipality.rejected,
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

export const selectMunicipalities = (
    state: {
        municipalities: MunicipalityState
    }
) => state.municipalities.items;

export const {
    clearSelected,
    clearError

} = municipalitySlice.actions;



export default municipalitySlice.reducer;
