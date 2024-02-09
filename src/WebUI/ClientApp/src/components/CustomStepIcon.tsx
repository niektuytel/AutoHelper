import React from "react";
import clsx from "clsx";
import { StepIconProps } from "@mui/material/StepIcon";
import Check from "@mui/icons-material/Check";

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
        fontSize: 18
    },
}

interface IProps {
    props: StepIconProps,
    index:number
}

export default ({props, index}:IProps) => {
    const { active, completed } = props;
  
    return <>
        <div className={clsx(styles.root, { [styles.active.color]: active })}>
            {completed ? 
                <Check 
                    sx={styles.completed} 
                    style={{ color: 'red' }} 
                /> 
            :   
                <div style={{ ...styles.circle, color:'red'}}/>
            }
        </div>
    </>
}

