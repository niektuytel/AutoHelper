import React from "react"
import { Snackbar } from "@mui/material";
import { Alert } from "@mui/lab";
import { useTranslation } from "react-i18next"
import { useDispatch, useSelector } from "react-redux"
// import { setErrorStatus, setSuccessStatus } from "../../store/status/StatusActions"
//import StatusState from "../../store/status/StatusState"

export default () => {
    const { t } = useTranslation();
    // const dispatch = use// dispatch();
    //const { error_message, success_message }:StatusState = useSelector((state:any) => state.status)
    const error_message = "";
    const success_message = "";
    
    const onCloseError = () => {
        // dispatch(setErrorStatus(""))
    }
    
    const onCloseSuccess = () => {
        // dispatch(setSuccessStatus(""))
    }
    
    return <>
        <Snackbar 
            open={success_message.length > 0} 
            autoHideDuration={6000} 
            onClose={onCloseSuccess}
        >
            <Alert onClose={onCloseSuccess} severity="success">
                {t("successful_action")}
            </Alert>
        </Snackbar>
        <Snackbar 
            open={error_message.length > 0} 
            autoHideDuration={6000} 
            onClose={onCloseError}
        >
            <Alert onClose={onCloseError} severity="error">
                {error_message}
                {/* t("failed_action") */}
            </Alert>
        </Snackbar>
    </>
}