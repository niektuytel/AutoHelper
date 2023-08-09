import React from 'react';
import { InputLabel, MenuItem, Select } from '@material-ui/core';
import { createStyles, makeStyles } from '@material-ui/core';

const styles = makeStyles(() =>
    createStyles({
        inputLabel: {
            textAlign:"left"
        },
        selectBox: {
            width:"100%"
        }
    }),
);

interface IProps {
    title: string;
    data:any[];
    value: any;
    setValue: (value:any) => void;
}
  
export default ({title, data, value, setValue}:IProps) => {
    const classes = styles();
    const [openSelect, setOpenSelect] = React.useState(false);
    
    const handleOpenSelect = () => {
        setOpenSelect(true);
    };
    
    const handleCloseSelect = () => {
        setOpenSelect(false);
    };
    
    return <>
        <InputLabel id="label" className={classes.inputLabel}>{title}</InputLabel>
        <Select
            labelId="label"
            className={classes.selectBox}
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
