import React from  "react";
import { Box, Paper, TextareaAutosize, TextField } from "@material-ui/core";
import ControlButtons from "../../../components/control_buttons/ControlButtons";
import EditTextPaperStyle from "./EditTextPaperStyle";
import EditText from "../../../components/textfields/EditText";

interface IProps {
    label:string;
    value:string|undefined;
    setValue: (value:string) => void;
    onSave: () => void;
    isAdmin:boolean;
    multilines?:boolean;
}

export default ({label, value, setValue, onSave, isAdmin, multilines}:IProps) => {
    const classes = EditTextPaperStyle();
    
    return <>
        <Paper className={classes.paper}>
            <EditText 
                label={label} 
                value={value} 
                setValue={setValue}
                multilines={multilines}
            />
            <Box className={classes.control_button}>
                <ControlButtons 
                    onSave={onSave} 
                    isAdmin={isAdmin}
                    containStyle={false}
                    submitOn="save"
                />
            </Box>
        </Paper>
    </>
}