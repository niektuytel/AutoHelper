import React from "react";
import { Snackbar, SnackbarCloseReason } from "@mui/material";
import { Alert } from "@mui/lab";
import { useDispatch, useSelector } from "react-redux";
import { closeSnackbar } from "../redux/slices/statusSnackbarSlice";

export default () => {
    const dispatch = useDispatch();
    const { message, type, open } = useSelector((state: any) => state.statusSnackbar);

    const handleClose = (event: Event | React.SyntheticEvent, reason?: SnackbarCloseReason) => {
        if (reason === 'clickaway') {
            return;
        }

        dispatch(closeSnackbar());
    };

    return (
        <Snackbar
            open={open}
            autoHideDuration={6000}
            onClose={handleClose}
            anchorOrigin={{ vertical: 'top', horizontal: 'left' }}
        >
            <Alert onClose={handleClose} severity={type}>
                {message}
            </Alert>
        </Snackbar>
    );
}
