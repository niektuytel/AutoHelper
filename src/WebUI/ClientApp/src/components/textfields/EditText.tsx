import React from  "react";
import { Box, TextareaAutosize, TextField } from "@mui/material";

const styles = {
    paper: {
        padding: "10px",
        marginBottom: "20px",
        display: 'flex',
        flexDirection: 'column',
        textAlign: "center"
    },
    text_field: {
        width: "100%",
        marginTop: "10px"
    },
    control_button: {
        textAlign: "right",
        marginTop: "10px"
    }
}

interface IProps {
    label:string;
    value:string|undefined;
    setValue: (value:string) => void;
    multilines?:boolean;
    isNumber?:boolean;
    helperText?:string;
}

export default ({label, value, setValue, multilines, isNumber, helperText}:IProps) => {
    const onChange = (event:any) => {
        setValue(event.target.value);
    }

    return <>
        {multilines ? 
            <Box>
                <TextareaAutosize
                    placeholder={label}
                    style={{...styles.text_field, minHeight:"25vh", minWidth:"300px", maxWidth:"400px"}}
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
                    sx={styles.text_field}
                    value={value}
                    helperText={helperText}
                    onChange={onChange}
                />
            :
                <TextField 
                    id="outlined-basic" 
                    label={label}
                    variant="outlined" 
                    sx={styles.text_field}
                    value={value}
                    helperText={helperText}
                    onChange={onChange}
                />
        }
    </>
}