import {
    createAsyncThunk,
    createSlice
} from "@reduxjs/toolkit";


import {
    RegionTypeRequest,
    RegionTypeResponse,
    CreateRegionTypeResponse,
    PaginatedRegionTypeResponse
} from "./regionType.types";


import {
    getRegionTypes,
    getRegionTypeByCode,
    createRegionType,
    updateRegionType,
    deleteRegionType,
} from "./regionType.service";





interface RegionTypeState {

    items: RegionTypeResponse[];

    selected: RegionTypeResponse | null;

    totalCount: number;

    loading: boolean;

    error: string | null;

}




const initialState: RegionTypeState = {

    items: [],

    selected: null,

    totalCount: 0,

    loading: false,

    error: null,

};





export const fetchRegionTypes = createAsyncThunk<
    PaginatedRegionTypeResponse,
    {
        pageIndex: number;
        pageSize: number;
        search?: string;
    },
    {
        rejectValue: string;
    }
>(

    "regionTypes/fetchAll",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await getRegionTypes(
                request.pageIndex,
                request.pageSize,
                request.search
            );

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to fetch regionTypes"
            );

        }

    }

);


export const fetchRegionTypeByKey = createAsyncThunk<
    RegionTypeResponse,
    string,
    {
        rejectValue: string;
    }
>(

    "regionTypes/getByKey",

    async (
        code,
        { rejectWithValue }
    ) => {

        try {

            return await getRegionTypeByCode(code);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to get regionType"
            );

        }

    }

);


export const createNewRegionType = createAsyncThunk<
    CreateRegionTypeResponse,
    RegionTypeRequest,
    {
        rejectValue: string;
    }
>(

    "regionTypes/create",

    async (
        request,
        { rejectWithValue }
    ) => {

        try {

            return await createRegionType(request);

        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to create regionType"
            );

        }

    }

);


export const updateExistingRegionType = createAsyncThunk<
    string,
    {
        code: string;
        request: RegionTypeRequest;
    },
    {
        rejectValue: string;
    }
>(

    "regionTypes/update",

    async (
        { code, request },

        { rejectWithValue }

    ) => {

        try {


            await updateRegionType(
                code,
                request
            );


            return code;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to update regionType"
            );

        }

    }

);


export const deleteExistingRegionType = createAsyncThunk<
    string,
    string,
    {
        rejectValue: string;
    }
>(

    "regionTypes/delete",

    async (
        code,
        { rejectWithValue }
    ) => {

        try {

            await deleteRegionType(code);


            return code;


        }
        catch (error: unknown) {

            if (error instanceof Error)
                return rejectWithValue(error.message);


            return rejectWithValue(
                "Failed to delete regionType"
            );

        }

    }

);


const regionTypeSlice = createSlice({

    name: "regionTypes",

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
                fetchRegionTypes.pending,
                state => {

                    state.loading = true;
                    state.error = null;

                }
            )
            .addCase(
                fetchRegionTypes.fulfilled,
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
                fetchRegionTypes.rejected,
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
                fetchRegionTypeByKey.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                fetchRegionTypeByKey.fulfilled,
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
                fetchRegionTypeByKey.rejected,
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
                createNewRegionType.pending,
                state => {

                    state.loading = true;

                    state.error = null;

                }
            )
            .addCase(
                createNewRegionType.fulfilled,
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
                createNewRegionType.rejected,
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
                updateExistingRegionType.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                updateExistingRegionType.fulfilled,
                state => {

                    state.loading = false;

                }
            )
            .addCase(
                updateExistingRegionType.rejected,
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
                deleteExistingRegionType.pending,
                state => {

                    state.loading = true;

                }
            )
            .addCase(
                deleteExistingRegionType.fulfilled,
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
                deleteExistingRegionType.rejected,
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

export const selectRegionTypes = (
    state: {
        regionTypes: RegionTypeState
    }
) => state.regionTypes.items;

export const {
    clearSelected,
    clearError

} = regionTypeSlice.actions;



export default regionTypeSlice.reducer;
