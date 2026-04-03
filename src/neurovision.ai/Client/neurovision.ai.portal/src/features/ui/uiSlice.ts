import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface UiState {
    message: string;
    type: "success" | "error" | "info" | "warning" | null;
    visible: boolean;
}

const initialState: UiState = {
    message: "",
    type: null,
    visible: false,
};

const uiSlice = createSlice({
    name: "ui",
    initialState,
    reducers: {
        showAlert(state, action: PayloadAction<{ message: string; type: UiState["type"] }>) {
            state.message = action.payload.message;
            state.type = action.payload.type;
            state.visible = true;
        },
        hideAlert(state) {
            state.visible = false;
        },
    },
});

export const { showAlert, hideAlert } = uiSlice.actions;
export default uiSlice.reducer;