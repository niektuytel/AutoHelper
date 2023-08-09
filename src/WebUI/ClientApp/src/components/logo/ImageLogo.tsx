import React from "react";
import { makeStyles } from "@material-ui/core";
import { useHistory } from "react-router";

const ImageIconStyle = makeStyles(() => ({
    root: {
        "&:hover": {
            // background: "#efefef",
            cursor: "pointer"
        }
    }
})) 

interface IProps {
    small?: boolean;
    large?: boolean;
    very_large?: boolean;
    className?: string;
}

export default ({ small, large, very_large, className }:IProps) => {
    const history = useHistory();
    const classes = ImageIconStyle();
    const size:string = small ? "32px" : large ? "70px" : very_large ? "140px" : "60px";
    
    const onClick = () => {
        history.push("/");
    }

    return <>
        <img 
            src="/images/ic_blue_autohelper.svg" 
            height={size}
            className={`${classes.root} ${className || ''}`}
            onClick={onClick}
            alt="AutoHelper.nl"
        />
    </>
}