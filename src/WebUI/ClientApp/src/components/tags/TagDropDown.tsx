import React from 'react';
import { Box, InputLabel, MenuItem, Select } from '@material-ui/core';
import ITag from '../../interfaces/tag/ITag';
import ProgressBox from '../progress/ProgressBox';
import TagDropDownStyle from './TagDropDownStyle';

interface IProps {
    tags:ITag[]|undefined;
    value: ITag;
    setValue: (value:ITag) => void;
}
  
export default ({tags, value, setValue}:IProps) => {
    const classes = TagDropDownStyle();
    const [openSelect, setOpenSelect] = React.useState(false);
    
    const handleOpenSelect = () => {
        setOpenSelect(true);
    };
    
    const handleCloseSelect = () => {
        setOpenSelect(false);
    };
    
    console.log(value);

    return <>
        {!tags ?
            <ProgressBox/>
            :
            <Box className={classes.width}>
                <InputLabel id="label" className={classes.inputLabel}>Tag</InputLabel>
                <Select
                    labelId="label"
                    className={classes.selectBox}
                    value={value.id}
                    open={openSelect}
                    onOpen={handleOpenSelect}
                    onClose={handleCloseSelect}
                    onChange={(event:any) => setValue(tags.filter(tag => tag.id === event.target.value)[0])}
                >
                    {tags && tags.map((tag:ITag, index:number) => 
                        <MenuItem key={`menu-${index}`} value={tag.id}>{tag.name}</MenuItem>
                    )}
                </Select>
            </Box>
        }
    </>
}
