import React from "react";
import { Box, Button, Divider } from "@material-ui/core";
import { useDispatch, useSelector } from "react-redux";

import { setCompletedSteps, setCurrentStep, setUsedFilterSituation } from "../../../store/tag_filtering/TagFilteringActions";
import ActiveSituationState from "../../../store/tag_filtering/TagFilteringState";
import { requestCardProducts } from "../../../store/products/ProductsActions";
import { getNextFilters } from "../../../store/tag_filter/TagFilterActions";
import TagFilterState from "../../../store/tag_filter/TagFilterState";
import FilteringSituations from "./FilteringSituations";
import FilteringStepper from "./FilteringStepper";
import TagResults from "./TagResults";
import ITagFilterSituation from "../../../interfaces/tag/ITagFilterSituation";
import ProgressBox from "../../../components/progress/ProgressBox";
import { useTranslation } from "react-i18next";
import FilteringSectionStyle from "../sections/FilteringSectionStyle";
import TagItems from "../../../components/tags/TagItems";

interface IProps {
    setSortOnTags: (tags: string[]) => void;
    sortOnPopularity: boolean;
    sortOnPrice: boolean;
}

export default ({setSortOnTags, sortOnPopularity, sortOnPrice}:IProps) => {
    const classes = FilteringSectionStyle();
    const dispatch = useDispatch();
    const {t} = useTranslation();
    const {isLoading, filterQuestions}: TagFilterState = useSelector((state:any) => state.tag_filters);
    const {target, currentStep, completedSteps, usedSituations}: ActiveSituationState = useSelector((state:any) => state.tag_filtering);

    const setCurrentSituations = (situations:{[k: number]: ITagFilterSituation;}) => {
        if(!filterQuestions) return;

        var situation = situations[filterQuestions[currentStep].id];
        if(situation)
        {
            const newCompleted = completedSteps;
            newCompleted[currentStep] = true;
            dispatch(setCompletedSteps(newCompleted));
        }
        
        // set tags
        let tags:string[] = [];
        Object.entries(situations).forEach(([key, value]) => 
            value.missingItems.forEach((tag) =>  
                tags.push(tag.name)
            )
        );

        dispatch(setUsedFilterSituation(situations));
        setSortOnTags(tags);
        dispatch(requestCardProducts("TODO: Admin Azure account ID from azure!!!", 0, tags, sortOnPopularity, sortOnPrice));
    };

    const handleToNext = () => {
        if(!filterQuestions || !target) return;

        const situations = Object.values(usedSituations);
        dispatch(getNextFilters(target, situations));
        
        let stepIndex = ((currentStep + 1) % filterQuestions.length);
        dispatch(setCurrentStep(stepIndex));
    };

    const handleToPrev = () => {
        let stepIndex = (currentStep - 1);
        dispatch(setCurrentStep(stepIndex));
        
        // dispatch({ type: 'TAG_FILTER_QUESTION_RESPONSE', filterQuestions: filterQuestions.slice(0, currentStep) });
    };
    
    return <>
        {filterQuestions && !isLoading ? 
            <>
                <FilteringStepper filters={filterQuestions}/>
                <FilteringSituations 
                    filter={filterQuestions[currentStep]}
                    setCurrentSituations={setCurrentSituations}
                />
            </>
            :
            <ProgressBox/>
        }
        <Divider variant="middle"/>
        <TagResults/>
        {/* <TagItems data={[]}/> */}
        <Box className={classes.btn_box}>
            <Button 
                disabled={currentStep <= 0}
                className={classes.btn}
                variant="outlined" 
                onClick={handleToPrev}
            >
                {t("back")}
            </Button>
            <Button 
                disabled={filterQuestions ? (currentStep >= (filterQuestions.length-1)) : true}
                className={classes.btn}
                variant="outlined" 
                onClick={handleToNext}
            >
                {t("next")}
            </Button>
        </Box>
    </>
}