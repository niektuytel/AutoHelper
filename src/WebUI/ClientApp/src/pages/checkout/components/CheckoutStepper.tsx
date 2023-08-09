import React from "react";
import { makeStyles, Step, StepLabel, Stepper, Typography } from "@material-ui/core";
import CustomStepIcon from "../../../components/step/CustomStepIcon";
import { useTranslation } from "react-i18next";


const CheckoutStepperStyle = makeStyles(() => ({
    stepper: { paddingTop: 3, paddingBottom: 5, color:"black" },

}))

interface IProps {
    currentStep:number;
    onError:boolean;
}

export default ({currentStep, onError}:IProps) => {
    const {t} = useTranslation();
    const classes = CheckoutStepperStyle();
    const steps = [ t("shipping_address"), t("payment"), t("thank_you") ]

    const interactOnError = (index:number, valid:any, invalid:any) => {
        if(currentStep === index)
        {
            if(onError)
            {
                return invalid;
            }
            return valid;
        }
        return valid;
    }

    return <>
        <Typography component="h1" variant="h4" align="center">
            {onError ? t("error") : steps[currentStep]}
        </Typography>
        <Stepper activeStep={currentStep} className={classes.stepper} alternativeLabel>
            {steps.map((label:string, index:number) => (
                <Step key={label}>
                    <StepLabel 
                        StepIconComponent={interactOnError(index, (props:any) => <CustomStepIcon props={props} index={index}/>, undefined)}
                        error={currentStep === index ? onError : false} 
                    >
                        {interactOnError(index, label, t("error"))}
                    </StepLabel>
                </Step>
            ))}
        </Stepper>
    </>;
}