import React from 'react';
import InputLabel from '@mui/material/InputLabel';
import MenuItem from '@mui/material/MenuItem';
import Select from '@mui/material/Select';


const styles = {
        inputLabel: {
            textAlign:"left"
        },
        selectBox: {
            width:"100%"
        }
}

interface IProps {
    title: string;
    data:any[];
    value: any;
    setValue: (value:any) => void;
}
  
export default ({title, data, value, setValue}:IProps) => {
    const [openSelect, setOpenSelect] = React.useState(false);
    
    const handleOpenSelect = () => {
        setOpenSelect(true);
    };
    
    const handleCloseSelect = () => {
        setOpenSelect(false);
    };
    
    return <>
        <InputLabel id="label" sx={styles.inputLabel}>{title}</InputLabel>
        <Select
            labelId="label"
            sx={styles.selectBox}
            value={value ? value : ""}
            open={openSelect}
            onOpen={handleOpenSelect}
            onClose={handleCloseSelect}
            onChange={(event:any) => setValue(data.filter(elem => elem.subject === event.target.value)[0].subject)}
        >
            {data && data.map((elem:any, index:number) => 
                <MenuItem key={`menu-${index}`} value={elem.subject}>{elem.subject}</MenuItem>
            )}
        </Select>
    </>
}
