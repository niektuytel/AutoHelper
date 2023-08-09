import React, { useEffect, useState } from 'react';
import { Box, InputLabel, MenuItem, Select } from '@material-ui/core';
import TagTargetDropDownStyle from './TagTargetDropDownStyle';
import ITagFilterTarget, { emptyFilterTarget } from '../../../interfaces/tag/ITagFilterTarget';
import { httpGetTagFilterTargets } from '../../../services/TagFilterService';
import { useDispatch } from 'react-redux';
import { setErrorStatus } from '../../../store/status/StatusActions';
import ProgressBox from '../../../components/progress/ProgressBox';

interface IProps {
    targetId: number;
    setTargetId: (value:number) => void;
}
  
export default ({targetId, setTargetId: setValue}:IProps) => {
    const classes = TagTargetDropDownStyle();
    const dispatch = useDispatch();
    const [openSelect, setOpenSelect] = useState(false);
    const [loading, setLoading] = useState<boolean|undefined>(undefined);
    const [targets, setTargets] = useState<ITagFilterTarget[]>([]);

    let items = targets.filter(item => item.id === targetId);
    let target:ITagFilterTarget = emptyFilterTarget; 
    if(items.length > 0) target = items[0];
    
    useEffect(() => {
        if(loading === undefined)
        {
            setLoading(true);
            httpGetTagFilterTargets(
                (data:ITagFilterTarget[]) => {
                    setLoading(false);
                    setTargets(data);
                },
                (message:string) => {
                    setLoading(false);
                    dispatch(setErrorStatus(message));
                }
            )
        }
    });
    
    const handleOpenSelect = () => {
        setOpenSelect(true);
    };
    
    const handleCloseSelect = () => {
        setOpenSelect(false);
    };
    
    return <>
        {!loading && targets ?
            <Box className={classes.width}>
                <InputLabel id="label" className={classes.inputLabel}>Target</InputLabel>
                <Select
                    labelId="label"
                    className={classes.selectBox}
                    value={target.id < 1 ? "" : target.id}
                    open={openSelect}
                    onOpen={handleOpenSelect}
                    onClose={handleCloseSelect}
                    onChange={(event:any) => setValue(event.target.value)}
                >
                    {targets && targets.map((target:ITagFilterTarget, index:number) => 
                        <MenuItem key={`menu-${index}`} value={target.id}>{target.title}</MenuItem>
                    )}
                </Select>
            </Box>
            :
            <ProgressBox/>
        }
    </>
}
