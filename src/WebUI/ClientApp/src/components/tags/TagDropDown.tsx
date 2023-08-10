import React from 'react';
import { Box, InputLabel, MenuItem, Select } from '@mui/material';
import ITag from '../../interfaces/tag/ITag';
import ProgressBox from '../progress/ProgressBox';

const styles = {
    dialog: {
        width: "400px"
    },
    width: {
        width: "100%"
    },
    inputLabel: {
        textAlign: "left"
    },
    selectBox: {
        width: "100%"
    },
    percentageBox: {
        margin: "30px",
        textAlign: "center"
    }
}

interface IProps {
    tags:ITag[]|undefined;
    value: ITag;
    setValue: (value:ITag) => void;
}
  
export default ({tags, value, setValue}:IProps) => {
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
            <Box sx={styles.width}>
                <InputLabel id="label" sx={styles.inputLabel}>Tag</InputLabel>
                <Select
                    labelId="label"
                    sx={styles.selectBox}
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
