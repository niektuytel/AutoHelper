import React from "react";
import { Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";



export const styles = {
    root: {
        color:"black", 
        fontFamily:"Dubai light",//"'Nunito', sans-serif",
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
    const variant = small ? "h4" : large ? "h3" : very_large ? "h2" : "h5";
    
    const onClick = () => {
        navigate("/");
    }

    return <>
        <Typography variant={variant} sx={styles.root} onClick={onClick}>
            AutoHelper
        </Typography>
    </>
}