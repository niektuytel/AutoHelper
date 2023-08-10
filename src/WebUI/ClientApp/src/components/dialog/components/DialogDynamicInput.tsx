import React, { ReactNode, useState } from 'react';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import Typography from '@mui/material/Typography';
import DialogTitle from '@mui/material/DialogTitle';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import Grid from '@mui/material/Grid';
import Switch from '@mui/material/Switch';
import FormControlLabel from '@mui/material/FormControlLabel';
import InputLabel from '@mui/material/InputLabel';
import Select from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import IconButton from '@mui/material/IconButton';
import { styled, useTheme } from '@mui/material/styles';
import PhotoCamera from '@mui/icons-material/PhotoCamera';
import { useTranslation } from 'react-i18next';
import EditText from '../../textfields/EditText';
import TagDropDown from '../../tags/TagDropDown';
import ITag from '../../../interfaces/tag/ITag';
//import TagTargetDropDown from '../../../pages/dashboard/components/TagTargetDropDown';
//import TagFilterDropDown from '../../../pages/dashboard/components/TagFilterDropDown';
import INoteOnKey from '../../../interfaces/INoteOnKey';
import IProductSpecLine from '../../../interfaces/product/IProductSpecLine';
import DialogDropDown from './DialogDropDown';


const randomNumberInRange = (min:number, max:number) => {
    // 👇️ get number between min (inclusive) and max (inclusive)
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

const ignoreKey = (key:string, ignoredKeys?:string[]) => {
    if(ignoredKeys && ignoredKeys.includes(key)) return true;
    if(key === "id") return true;
    return false;
}

const isObject = (input:any) => {
    return typeof(input) === "object";
}

const isBoolean = (input:any) => {
    return typeof(input) === "boolean";
}

const isText = (input:any) => {
    return typeof(input) === "string";
}

const isNumber = (input:any) => {
    return typeof(input) === "number";
}

const useStyles = {
    input: {
      display: 'none',
    },
}

interface IProps {
    title:string;
    data: any;
    setData: (value:any) => void;
    tags?: ITag[];
    specifications?: IProductSpecLine[];
    onAttachFile?: (file:any) => void;
    multilines?: string[];
    ignoredKeys?: string[];
    noteOnKeys?: INoteOnKey[]
}
  
export default ({
    title, 
    data, 
    setData,
    tags, 
    specifications,
    onAttachFile, 
    multilines, 
    ignoredKeys, 
    noteOnKeys

}:IProps) => {
    const [randint, setRandint] = useState(randomNumberInRange(1, 1000000));

    const updateData = (key:string, value:any) => {
        let new_data = {...data};
        new_data[key] = value;
        setData(new_data);
    }
    
    const onImageChange = (event:any) => {
        if(!onAttachFile) return;
        if (event.target.files && event.target.files[0]) {
            onAttachFile(event.target.files[0]);
        }
    }

    const camelCaseToText = (value:string):string => {
        var regex = new RegExp(/([A-Z])\w+/);
        var content = regex.exec(value);
        if(content == null) return value;

        var char = content[1]
        value = value.replace(char, ` ${char.toLowerCase()}`)
        return value;
    }

    const getNoteOnKey = (key:string) => 
    {
        if(!noteOnKeys) return undefined;

        var matches = noteOnKeys.filter(item => item.key.toLowerCase() === key);
        if(matches && matches.length > 0)
        {
            return matches[0].message;
        }

        return undefined;
    }
    
    return <>
        <DialogContent dividers>
            {data && Object.entries(data).map((item:any, index:number) => {
                if (ignoreKey(item[0], ignoredKeys)) {
                    return <div key={`a-${randint}`}></div>
                }
                
                var text = camelCaseToText(item[0]);
                var key = String(item[0]).toLowerCase();
                var note = getNoteOnKey(key);

                if (key === "filtertargetid")
                {
                    //return <TagTargetDropDown 
                    //    key={`${title}-targetdropdown-${index}-${randint}`} 
                    //    targetId={item[1]} 
                    //    setTargetId={(value:number) => updateData(item[0], value)}
                    ///>
                }
                else if (key === "filterid")
                {
                    //return <TagFilterDropDown 
                    //    key={`${title}-filterdropdown-${index}-${randint}`} 
                    //    filterId={item[1]} 
                    //    setFilterId={(value:number) => updateData(item[0], value)}
                    ///>
                }
                else if (key === "childof" && specifications)
                {
                    if(specifications.length > 0)
                    {
                        return <DialogDropDown
                            key={`${title}-dialogdropdown-${index}-${randint}`} 
                            title={key}
                            data={specifications} 
                            value={item[1]} 
                            setValue={(value:any) => updateData(item[0], value)}                        
                        />
                    }
                }
                else if (isObject(item[1]) && tags)
                {
                    if(tags.length > 0)
                    {
                        return <TagDropDown 
                            key={`${title}-dropdown-${index}-${randint}`} 
                            tags={tags} 
                            value={item[1]} 
                            setValue={(data:ITag) => updateData(item[0], data)}
                        />
                    }
                }
                else if (isBoolean(item[1]))
                {                    
                    return <FormControlLabel
                        key={`${title}-formcontrollabel-${index}-${randint}`} 
                        labelPlacement="start"
                        label={text}
                        value={item[1]}
                        control={
                            <Switch 
                                color="primary"
                                checked={item[1]}
                                onChange={(event:any) => updateData(item[0], !item[1])} 
                            />
                        }
                    />
                }
                else if(item[0].includes("image") && onAttachFile)
                {
                    return <Grid 
                        key={`${title}-grid-${index}-${randint}`} 
                        container 
                        spacing={3}
                    >
                        <Grid item xs={3} style={{marginTop:"10px", marginBottom:"10px"}}>
                            <Typography gutterBottom>{item[0]}</Typography>
                        </Grid>
                        <Grid item xs={9}>
                            <input 
                                id="icon-button-file" 
                                style={useStyles.input} 
                                accept="image/*" type="file"
                                onChange={(event:any) => onImageChange(event)}
                            />
                            <label htmlFor="icon-button-file">
                                <IconButton color="primary" aria-label="upload picture" component="span">
                                    <PhotoCamera />
                                </IconButton>
                            </label>
                        </Grid>
                    </Grid>
                }
                else if (isText(item[1]) || isNumber(item[1]))
                {
                    return <EditText 
                        key={`${title}-edittext-${index}-${randint}`} 
                        isNumber={isNumber(item[1])}
                        label={text} 
                        value={item[1]} 
                        helperText={note}
                        setValue={(value:string) => updateData(item[0], value)}   
                        multilines={multilines !== undefined && multilines.includes(item[0])}                                 
                    />
                }
                
                return <div key={`${randint}`}></div>
            })}
        </DialogContent>
    </>
}