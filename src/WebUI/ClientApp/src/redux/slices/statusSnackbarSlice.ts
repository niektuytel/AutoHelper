import { createSlice } from "@reduxjs/toolkit";

const initialState = {
    message: "",
    type: "success", // or "error"
    open: false,
};

const statusSnackbarSlice = createSlice({
    name: "statusSnackbar",
    initialState,
    reducers: {
        showOnSuccess: (state, action) => {
            state.type = "success";
            state.message = action.payload;
            state.open = true;
        },
        showOnError: (state, action) => {
            state.type = "error";
            state.message = action.payload;
            state.open = true;
        },
        closeSnackbar: (state) => {
            state.open = false;
            state.message = "";
        },
    },
});

export const { showOnSuccess, showOnError, closeSnackbar } = statusSnackbarSlice.actions;
export default statusSnackbarSlice.reducer;