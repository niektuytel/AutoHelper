import React from "react";
import { Typography } from "@mui/material";
import { useLocation, useNavigate } from "react-router-dom";



export const styles = {
    root: {
        color:"black", 
        fontFamily: "Dubai, sans-serif",
        fontWeight: 100,
        fontStyle: "normal",
        cursor:"pointer",
        marginTop:"5px"
    }
}

interface IProps {
    small?: boolean;
    large?: boolean;
    very_large?: boolean;
}

export default ({small, large, very_large}:IProps) => {
    const navigate = useNavigate();
    const location = useLocation();
    const variant = small ? "h4" : large ? "h3" : very_large ? "h2" : "h5";
    
    const onClick = () => {
        navigate("/", { state: { from: location } });
    }

    return <>
        <Typography variant={variant} sx={styles.root} onClick={onClick}>
            AutoHelper
        </Typography>
    </>
}