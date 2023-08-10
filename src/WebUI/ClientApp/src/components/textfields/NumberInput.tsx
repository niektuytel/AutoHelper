import React, { CSSProperties } from "react";
import { TextField, Typography } from "@mui/material"

interface IProps {
    label: string;
    value: number;
    setValue: (value:number) => void;
}

const LabelStyle = () : CSSProperties => ({ 
    display: "inline-flex",
    marginTop: "2px",
    marginRight: "5px"  
})

const LabelTextFieldStyle = () : CSSProperties => ({
    maxWidth:"150px", 
    textAlignLast:"center",
    marginRight:"20px"
})

export default ({label, value, setValue}:IProps) => {
    return <>
        <Typography variant="body1" style={LabelStyle()}>
            {label}
        </Typography>
        <TextField
            autoFocus
            name="numberformat"
            variant="outlined"
            style={LabelTextFieldStyle()}
            inputProps={{ style: { padding: 5 } }}
            value={value}
            onChange={(event:any) => setValue(event.target.value)}
        />
    </>
}

