import React from "react";
import clsx from "clsx";
import { StepIconProps } from "@mui/material/StepIcon";
import Check from "@mui/icons-material/Check";

//import CustomStepIconStyle from "./CustomStepIconStyle";
import { colorOnIndex } from "../../i18n/ColorValues";

const styles = {
    root: {
        color: '#eaeaf0',
        display: 'flex',
        height: 22,
        alignItems: 'center',
    },
    active: {
        color: '#784af4',
    },
    circle: {
        width: 10,
        height: 10,
        borderRadius: '50%',
        backgroundColor: 'currentColor',
    },
    completed: {
        zIndex: 1,
        fontSize: 18
    },
}

interface IProps {
    props: StepIconProps,
    index:number
}

export default ({props, index}:IProps) => {
    //const classes = CustomStepIconStyle();
    const { active, completed } = props;
  
    return <>
        <div className={clsx(styles.root, { [styles.active.color]: active })}>
            {completed ? 
                <Check 
                    sx={styles.completed} 
                    style={{color:colorOnIndex(index)}} 
                /> 
            :   
                <div style={{ ...styles.circle, color:colorOnIndex(index)}}/>
            }
        </div>
    </>
}

