import React from "react";
import { useNavigate } from "react-router-dom";

const styles = {
    root: {
        "&:hover": {
            // background: "#efefef",
            cursor: "pointer"
        }
    }
}

interface IProps {
    small?: boolean;
    large?: boolean;
    very_large?: boolean;
    className?: string;
}

export default ({ small, large, very_large, className }:IProps) => {
    const navigate = useNavigate();
    const size:string = small ? "32px" : large ? "70px" : very_large ? "140px" : "60px";
    
    const onClick = () => {
        navigate("/");
    }

    return <>
        <img 
            src="/images/ic_blue_autohelper.svg" 
            height={size}
            className={`${styles.root} ${className || ''}`}
            onClick={onClick}
            alt="AutoHelper.nl"
        />
    </>
}