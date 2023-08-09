import React, { useEffect, useState } from 'react';
import { Box, InputLabel, MenuItem, Select } from '@material-ui/core';
import TagTargetDropDownStyle from './TagTargetDropDownStyle';
import ITagFilterTarget, { emptyFilterTarget } from '../../../interfaces/tag/ITagFilterTarget';
import { httpGetTagFilters, httpGetTagFilterTargets } from '../../../services/TagFilterService';
import { useDispatch } from 'react-redux';
import { setErrorStatus } from '../../../store/status/StatusActions';
import ProgressBox from '../../../components/progress/ProgressBox';
import ITagFilter, { emptyTagFilter } from '../../../interfaces/tag/ITagFilter';

interface IProps {
    filterId: number;
    setFilterId: (value:number) => void;
}
  
export default ({filterId, setFilterId}:IProps) => {
    const classes = TagTargetDropDownStyle();
    const dispatch = useDispatch();
    const [openSelect, setOpenSelect] = useState(false);
    const [loading, setLoading] = useState<boolean|undefined>(undefined);
    const [filters, setFilters] = useState<ITagFilter[]>([]);

    let items = filters.filter(item => item.id === filterId);
    let filter:ITagFilter = emptyTagFilter; 
    if(items.length > 0) filter = items[0];
    
    useEffect(() => {
        if(loading === undefined)
        {
            setLoading(true);
            httpGetTagFilters(
                (data:ITagFilter[]) => {
                    setLoading(false);
                    setFilters(data);
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
        {!loading && filters ?
            <Box className={classes.width}>
                <InputLabel id="label" className={classes.inputLabel}>Filter</InputLabel>
                <Select
                    labelId="label"
                    className={classes.selectBox}
                    value={filter.id < 1 ? "" : filter.id}
                    open={openSelect}
                    onOpen={handleOpenSelect}
                    onClose={handleCloseSelect}
                    onChange={(event:any) => setFilterId(event.target.value)}
                >
                    {filters && filters.map((target:ITagFilter, index:number) => 
                        <MenuItem key={`menu-${index}`} value={target.id}>{target.title}</MenuItem>
                    )}
                </Select>
            </Box>
            :
            <ProgressBox/>
        }
    </>
}
