import { Box, Button, Paper } from "@material-ui/core";
import React, { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useTranslation } from "react-i18next";

import { getNextFilters } from "../../../store/tag_filter/TagFilterActions";
import FilteringTarget from "../components/FilteringTarget";
import Filtering from "../components/Filtering";
import ActiveSituationState from "../../../store/tag_filtering/TagFilteringState";
import FilteringSectionStyle from "./FilteringSectionStyle";

interface IProps {
    setSortOnTags: (tags: string[]) => void;
    sortOnPopularity: boolean;
    sortOnPrice: boolean;
}

export default ({setSortOnTags, sortOnPopularity, sortOnPrice}:IProps) => {
    const classes = FilteringSectionStyle();
    const {t} = useTranslation();
    const dispatch = useDispatch();
    const {target, usedSituations}: ActiveSituationState = useSelector((state:any) => state.tag_filtering);

    const [onTargetSlide, setOnTargetSlide] = useState<boolean>(target === undefined);
    const [disabledBtn, setDisabledBtn] = useState<boolean>(true);

    const nextFilter = () => {
        if(!target) return;
        setOnTargetSlide(false);
        setDisabledBtn(true);

        const situations = Object.values(usedSituations);
        dispatch(getNextFilters(target, situations));
    }
    
    return <>
        <Paper elevation={1} className={classes.paper}>
            { onTargetSlide ?
                <>
                    <FilteringTarget setDisableNext={setDisabledBtn}/>
                    <Box className={classes.btn_box}>
                        <Button 
                            disabled={disabledBtn}
                            className={classes.btn}
                            variant="outlined" 
                            onClick={nextFilter}
                        >
                            {t("start_filter")}
                        </Button>
                    </Box>
                </>
            :
                <Filtering 
                    setSortOnTags={setSortOnTags} 
                    sortOnPopularity={sortOnPopularity} 
                    sortOnPrice={sortOnPrice}
                />
            }
        </Paper>
    </>
}