import { Box, Typography } from "@mui/material";
import React from "react";
import ControlButtons from "../control_buttons/ControlButtons";

interface IProps
{
    title: string;
    text: string;
    onEdit: () => void;
    isAdmin: boolean | undefined
}

export default ({title, text, onEdit, isAdmin}:IProps) => {
    return <>
        <Box style={{ margin:"10px" }}>
            <Typography variant="h4" gutterBottom>
                {title}
            </Typography>
            <Typography variant="subtitle1" gutterBottom>
                {text}
            </Typography>
            <ControlButtons onEdit={onEdit} isAdmin={isAdmin}/>
        </Box>
    </>
}