import React from "react";
import { 
    createStyles, 
    makeStyles, 
    Step, 
    StepButton, 
    StepLabel, 
    Stepper, 
    Theme, 
    Typography 
} from "@material-ui/core";
import PersonalFilterState from "../../../store/tag_filtering/TagFilteringState";
import { useDispatch, useSelector } from "react-redux";
import { setCurrentStep } from "../../../store/tag_filtering/TagFilteringActions";
import CustomStepIcon from "../../../components/step/CustomStepIcon";
import ITagFilter from "../../../interfaces/tag/ITagFilter";

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    instructions: {
      marginBottom: theme.spacing(1),
    },
  }),
);

interface IProps {
    filters: ITagFilter[];
}

export default ({filters}:IProps) => {
    const classes = useStyles();
    const dispatch = useDispatch();
    const {currentStep, completedSteps}: PersonalFilterState = useSelector((state:any) => state.tag_filtering);

    const state = useSelector((state:any) => state);
    console.log(state);

    const handleStep = (step: number) => () => {
        dispatch(setCurrentStep(step));
    }
    
    if(filters.length === 0) return <></>
    return <>
        <Stepper nonLinear activeStep={currentStep}>
            {filters.map((filter:ITagFilter, index:number) => 
                <Step key={`question-step-${filter.id}`}>
                    <StepButton onClick={handleStep(index)} completed={completedSteps[index]} style={{padding:"20px"}}>
                        <StepLabel StepIconComponent={(props:any) => <CustomStepIcon props={props} index={index}/>}>
                            {filter.title}
                        </StepLabel>
                    </StepButton>
                </Step>
            )}
        </Stepper>
        <Typography variant="h5" className={classes.instructions}>
            <b>{filters[currentStep].title}</b>
        </Typography>
    </>;
}