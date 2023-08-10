import React from "react";
import { Checkbox, List, ListItem, ListItemIcon, ListItemText } from "@mui/material";
import { colorOnIndex } from "../../i18n/ColorValues";

interface IProps {
    labelName: string;
    labelHint?: string;
    data: any[];
    checked: any[];
    onCheck: (item: any) => () => void;
    noScroll?: boolean|undefined;
}

export default ({labelName, labelHint, data, checked, onCheck, noScroll}:IProps) => {
    const getValue = (data:any, props:string[]):any => {
        const prop = props.shift();
        if(!prop) return String(data);
        if(!data) return String(data);

        return getValue(data[prop], props);
    }

    const getHintLabel = (item:any) => {
        if(!labelHint) return "";

        const text = getValue(item, labelHint.split("."));
        if(text.length < 1) return "";

        return text
    }

    return <>
        <List dense component="div" role="list" style={noScroll ? {} : {maxHeight: '400px', overflow: 'auto'}}>
            {data && data.map((item:any, index:number) => 
                <ListItem 
                    key={index} 
                    role="listitem" 
                    button 
                    onClick={onCheck(item)} 
                >
                    <ListItemIcon>
                        <Checkbox 
                            style={{color:colorOnIndex(index)}} 
                            inputProps={{ 'aria-labelledby' : `list-item-${index}-label` }} 
                            checked={checked.indexOf(item) !== -1} 
                            tabIndex={-1} 
                            disableRipple 
                        />
                    </ListItemIcon>
                    <ListItemText 
                        id={`list-item-${index}-label`} 
                        primary={getValue(item, labelName.split("."))} 
                        secondary={getHintLabel(item)} 
                    />
                </ListItem>
            )}
            <ListItem />
        </List>
    </>
}