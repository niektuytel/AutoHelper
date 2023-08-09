import React from "react";
import clsx from "clsx";
import { StepIconProps } from "@material-ui/core";
import { Check } from "@material-ui/icons";

import CustomStepIconStyle from "./CustomStepIconStyle";
import { colorOnIndex } from "../../i18n/ColorValues";

interface IProps {
    props: StepIconProps,
    index:number
}

export default ({props, index}:IProps) => {
    const classes = CustomStepIconStyle();
    const { active, completed } = props;
  
    return <>
        <div className={clsx(classes.root, {[classes.active]: active})}>
            {completed ? 
                <Check 
                    className={classes.completed} 
                    style={{color:colorOnIndex(index)}} 
                /> 
            :   
                <div 
                    className={classes.circle} 
                    style={{color:colorOnIndex(index)}}
                />
            }
        </div>
    </>
}

