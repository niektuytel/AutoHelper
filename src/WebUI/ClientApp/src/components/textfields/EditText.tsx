import React from  "react";
import { Box, TextareaAutosize, TextField } from "@material-ui/core";
import EditTextStyle from "./EditTextStyle";

interface IProps {
    label:string;
    value:string|undefined;
    setValue: (value:string) => void;
    multilines?:boolean;
    isNumber?:boolean;
    helperText?:string;
}

export default ({label, value, setValue, multilines, isNumber, helperText}:IProps) => {
    const classes = EditTextStyle();

    const onChange = (event:any) => {
        setValue(event.target.value);
    }

    return <>
        {multilines ? 
            <Box>
                <TextareaAutosize
                    placeholder={label}
                    className={classes.text_field}
                    style={{minHeight:"25vh", minWidth:"300px", maxWidth:"400px"}}
                    value={value}
                    onChange={onChange}
                />
                {helperText}
            </Box>
            :
            isNumber ?
                <TextField 
                    id="outlined-basic" 
                    type="number"
                    label={label}
                    variant="outlined" 
                    className={classes.text_field}
                    value={value}
                    helperText={helperText}
                    onChange={onChange}
                />
            :
                <TextField 
                    id="outlined-basic" 
                    label={label}
                    variant="outlined" 
                    className={classes.text_field}
                    value={value}
                    helperText={helperText}
                    onChange={onChange}
                />
        }
    </>
}