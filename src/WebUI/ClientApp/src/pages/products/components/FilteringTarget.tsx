import { Box, Button, Fade, Grid, IconButton, Paper, Popper, PopperPlacementType, Typography } from "@material-ui/core";
import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";

import { setFilteringTarget } from "../../../store/tag_filtering/TagFilteringActions";
import ActiveSituationState from "../../../store/tag_filtering/TagFilteringState";
import { getFilterTargets } from "../../../store/tag_filter/TagFilterActions";
import TagFilterState from "../../../store/tag_filter/TagFilterState";
import ProgressBox from "../../../components/progress/ProgressBox";
import ITagFilterTarget from "../../../interfaces/tag/ITagFilterTarget";
import FilteringSectionStyle from "../sections/FilteringSectionStyle";
import { useTranslation } from "react-i18next";
import ContactSupportOutlinedIcon from '@material-ui/icons/ContactSupportOutlined';

interface IProps {
    setDisableNext: (value:boolean) => void;
}

export default ({setDisableNext}:IProps) => {
    const classes = FilteringSectionStyle();
    const dispatch = useDispatch();
    const {t} = useTranslation();

    const {isLoading, targets}: TagFilterState = useSelector((state:any) => state.tag_filters);
    const {target}: ActiveSituationState = useSelector((state:any) => state.tag_filtering);
    
    const [open, setOpen] = React.useState(false);
    const [placement, setPlacement] = React.useState<PopperPlacementType>();
    const [anchorEl, setAnchorEl] = React.useState<HTMLButtonElement | null>(null);

    useEffect(() => {
        if(isLoading === undefined)
        {
            dispatch(getFilterTargets());
        }
    });
    
    const handleClick = (newPlacement: PopperPlacementType) => (
        event: React.MouseEvent<HTMLButtonElement>,
    ) => {
        setAnchorEl(event.currentTarget);
        setOpen((prev) => placement !== newPlacement || !prev);
        setPlacement(newPlacement);
    };

    const getCustomBorderStyle = (data:ITagFilterTarget|undefined, id:number) => {
        if(!targets || !data) return;
        if(data.id === id)
        {
            // return `2px solid ${colorOnIndex(index)}`;
            return `3px solid rgba(0, 0, 0)`;
        }
        else
        {
            return `1px solid rgba(0, 0, 0, 0.12)`;
        }
    }

    const onPaperClick = (target:ITagFilterTarget) => {
        dispatch(setFilteringTarget(target));
        setDisableNext(false);
    }

    return <>
        <Box className={classes.title_box}>
            <Typography variant="h5" className={classes.instructions}>
                {t("select_your_profile")}
            </Typography>
            <IconButton 
                onClick={handleClick('bottom-start')}
                className={classes.info_icon}
            >
                <ContactSupportOutlinedIcon fontSize="small"/>
            </IconButton>
            <Popper open={open} anchorEl={anchorEl} placement={placement} transition>
                {({ TransitionProps }) => (
                <Fade {...TransitionProps} timeout={350}>
                    <Paper>
                    <Typography className={classes.typography}>{t("select_your_profile_explain")}</Typography>
                    </Paper>
                </Fade>
                )}
            </Popper>
        </Box>
        <Grid container style={{ justifyContent: "center" }}>
            {
                !isLoading ? targets && targets.map((item:ITagFilterTarget, index:number) => 
                    <Grid item key={`grid-${index}`} xs={12} lg={3} sm={4}>
                        <Paper 
                            variant="outlined"
                            elevation={0}
                            className={classes.paper_card}
                            onClick={() => onPaperClick(item)}
                            style={{ border: getCustomBorderStyle(target, item.id) }}
                        >
                            {item.title}<br/>
                            {item.description}<br/>
                            {item.gender}<br/>
                            {item.age}<br/>
                        </Paper>
                    </Grid>
                )
                :
                    <ProgressBox/>
            }
        </Grid>
    </>
}