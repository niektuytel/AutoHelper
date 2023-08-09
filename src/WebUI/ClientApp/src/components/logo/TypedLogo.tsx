import React from "react";
import { makeStyles, Typography } from "@material-ui/core";
import { useHistory } from "react-router";
import { Variant } from "@material-ui/core/styles/createTypography";


export const TypedIconStyle = makeStyles(() => ({
    root: {
        color:"black", 
        fontFamily:"Dubai light",//"'Nunito', sans-serif",
        cursor:"pointer",
        marginTop:"5px"
    }
})) 

interface IProps {
    small?: boolean;
    large?: boolean;
    very_large?: boolean;
}

export default ({small, large, very_large}:IProps) => {
    const history = useHistory();
    const classes = TypedIconStyle();
    const variant:Variant = small ? "h4" : large ? "h3" : very_large ? "h2" : "h5";
    
    const onClick = () => {
        history.push("/");
    }

    return <>
        <Typography variant={variant} className={classes.root} onClick={onClick}>
            AutoHelper
        </Typography>
    </>
}