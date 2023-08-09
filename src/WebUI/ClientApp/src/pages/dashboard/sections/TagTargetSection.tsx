import * as React from 'react';
import { useEffect, useState } from 'react';
import { Box, Grid } from '@material-ui/core';
import { useDispatch } from 'react-redux';

import ITag from '../../../interfaces/tag/ITag';
import { setErrorStatus } from '../../../store/status/StatusActions';
import TagPanelSectionStyle from './TagSectionStyle';
import TagTargetList from '../components/TagTargetList';
import ITagFilterTarget, { emptyFilterTarget } from '../../../interfaces/tag/ITagFilterTarget';
import ITagSupplement, { emptyTagSupplement } from '../../../interfaces/tag/ITagSupplement';
import { httpGetTags } from '../../../services/TagService';
import TagTargetSupplementList from '../components/TagTargetSupplementList';

export default () => {
    const dispatch = useDispatch();
    const classes = TagPanelSectionStyle();
    const [tags, setTags] = useState<ITag[]|undefined>(undefined);
    const [targets, setTargets] = useState<ITagFilterTarget[]|undefined>(undefined); 
    const [currentTarget, setCurrentTarget] = useState<ITagFilterTarget>(emptyFilterTarget); 
    const [currentSupplement, setCurrentSupplement] = useState<ITagSupplement>(emptyTagSupplement); 
    const [loading, setLoading] = useState<boolean|undefined>(undefined); 
    
    // Initialize
    useEffect(() => {
        if(loading === undefined)
        {
            setLoading(true);
            httpGetTags(
                (data:ITag[]) => {
                    setTags(data);
                    setLoading(false);
                },
                (message:string) => {
                    dispatch(setErrorStatus(message));
                }
            );
        }
    });

    return <>
        <Box className={classes.panel}>
            <Grid container spacing={3} className={classes.container}>
                <TagTargetList 
                    currentTarget={currentTarget} 
                    setCurrentTarget={setCurrentTarget} 
                    targets={targets} 
                    setTargets={setTargets}
                />
                <TagTargetSupplementList 
                    tags={tags} 
                    currentTarget={currentTarget} 
                    setCurrentTarget={setCurrentTarget} 
                    targets={targets} 
                    setTargets={setTargets} 
                    currentTargetSupplement={currentSupplement} 
                    setCurrentTargetSupplement={setCurrentSupplement}
                />
            </Grid>
        </Box>
    </>
      
}
