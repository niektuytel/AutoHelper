import { Box, CircularProgress, makeStyles } from "@material-ui/core";
import React from "react";


const useStyles = makeStyles(() => ({
    progress_parent:{
        textAlign:"center",
        padding:"50px"
    },
    progress : {
        color:"black"
    }
}));

interface IProps {
    fillScreen?: boolean|undefined;
}

export default ({fillScreen}:IProps) => {
    const classes = useStyles();
    var fill_style = {
        width:"100%", 
        height:"70vh", 
        padding:"10%",
    };

    return <>
        <Box className={classes.progress_parent} style={fillScreen ? fill_style : {}}>
            <CircularProgress className={classes.progress}/>
        </Box>
    </>
}