import { Box, CircularProgress, makeStyles } from "@mui/material";
import React from "react";


const useStyles = {
    progress_parent:{
        textAlign:"center",
        padding:"50px"
    },
    progress : {
        color:"black"
    }
};

interface IProps {
    fillScreen?: boolean|undefined;
}

export default ({fillScreen}:IProps) => {
    var fill_style = {
        width:"100%", 
        height:"70vh", 
        padding:"10%",
    };

    return <>
        <Box sx={useStyles.progress_parent} style={fillScreen ? fill_style : {}}>
            <CircularProgress sx={useStyles.progress}/>
        </Box>
    </>
}