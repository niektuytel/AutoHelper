import React from "react";
import { Box, makeStyles, Paper } from "@material-ui/core";

import PersonalFilterState from "../../../store/tag_filtering/TagFilteringState";
import { useSelector } from "react-redux";
import ITagFilterSituation from "../../../interfaces/tag/ITagFilterSituation";
import ITagFilter from "../../../interfaces/tag/ITagFilter";
import ITagFilterWithSituations from "../../../interfaces/tag/ITagFilterWithSituations";

const AnswersStyle = makeStyles(() => ({
    root: {
        display: "inline-table"
    },
    paper: {
        padding:"25px", 
        margin:"5px", 
        width:"500px",
        cursor: "pointer"
    }
}))

interface IProps {
    filter: ITagFilterWithSituations;
    setCurrentSituations: (answers:{ [k: number]: ITagFilterSituation }) => void;
}

export default ({filter: tagFilter, setCurrentSituations: setCurrentTagSituations}:IProps) => {
    const classes = AnswersStyle();
    const { usedSituations }: PersonalFilterState = useSelector((state:any) => state.tag_filtering);
    
    const setSituation = (situation:ITagFilterSituation) => {
        setCurrentTagSituations({ ...usedSituations, [tagFilter.id]:situation})
    }

    const getCustomBorderStyle = (data:ITagFilterSituation, index:number) => {
        if(!usedSituations) return;
        if(!usedSituations[tagFilter.id]) return;
        let answer = usedSituations[tagFilter.id];

        if(answer.id === data.id)
        {
            // return `2px solid ${colorOnIndex(index)}`;
            return `3px solid rgba(0, 0, 0)`;
        }
        else
        {
            return `1px solid rgba(0, 0, 0, 0.12)`;
        }
    }

    return <>
        <Box className={classes.root}>
            {tagFilter && tagFilter.situations.map(
                (situation:ITagFilterSituation, index:number) => 
                    <Paper 
                        key={`answer-${index}`}
                        elevation={3} 
                        variant="outlined" 
                        className={classes.paper}
                        onClick={() => setSituation(situation)}
                        style={{ border: getCustomBorderStyle(situation, index) }}
                    >
                        {situation.text}
                    </Paper>
            )}
        </Box>
    </>
}


