import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface UiState {
    message: string;
    type: "success" | "error" | "info" | "warning" | null;
    visible: boolean;
    id: number | null;
}

const initialState: UiState = {
    message: "",
    type: null,
    visible: false,
    id: null,
};

const uiSlice = createSlice({
    name: "ui",
    initialState,
    reducers: {
        showAlert(
            state,
            action: PayloadAction<{ message: string; type: UiState["type"] }>
        ) {
            state.message = action.payload.message;
            state.type = action.payload.type;
            state.visible = true;
            state.id = Date.now();
        },
        hideAlert(state) {
            state.visible = false;
            state.message = "";
            state.type = null;
            state.id = null;
        },
    },
});

export const { showAlert, hideAlert } = uiSlice.actions;
export default uiSlice.reducer;