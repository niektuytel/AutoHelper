import React, { useEffect, useState } from "react";
import { Box, Checkbox, Chip, FormControl, Input, InputLabel, ListItemText, MenuItem, Select } from "@material-ui/core";
import { t } from "i18next";

import ControlButtons from "../../../components/control_buttons/ControlButtons";
import TagStyle from "../../../components/tags/TagStyle";
import ITag from "../../../interfaces/tag/ITag";
import TagMultiSelectStyle from "./TagMultiSelectStyle";
import { httpGetTags } from "../../../services/TagService";
import { useDispatch } from "react-redux";
import { setErrorStatus } from "../../../store/status/StatusActions";
import ProgressBox from "../../../components/progress/ProgressBox";

const ITEM_HEIGHT = 48;
const ITEM_PADDING_TOP = 8;
const MenuProps = {
  PaperProps: {
    style: {
      maxHeight: ITEM_HEIGHT * 4.5 + ITEM_PADDING_TOP,
      width: 250,
    },
  },
};

interface IProps {
    default_data?: ITag[];
    missingItems: ITag[];
    changeTags: (tags:ITag[]) => void;
    onSaveTags?: (tags:ITag[]) => void;
}

export default ({default_data, missingItems, changeTags, onSaveTags}:IProps) => {
    const classes = TagMultiSelectStyle();
    const dispatch = useDispatch();

    const [loading, setLoading] = useState<boolean|undefined>(undefined); 
    const [originalTags, setOriginalTags] = useState<ITag[]|undefined>(default_data);
    const [tags, setTags] = useState<ITag[]>(missingItems);
    
    // Initialize
    useEffect(() => {
        if(loading === undefined && originalTags === undefined)
        {
            setLoading(true);
            httpGetTags(
                (data:ITag[]) => {
                    setOriginalTags(data);
                    setLoading(false);
                },
                (message:string) => {
                    dispatch(setErrorStatus(message));
                }
            );
        }
    });
    
    const handleChange = (event: React.ChangeEvent<{ value: unknown }>) => {
        if(!originalTags) return;

        let values = event.target.value as string[];
        let items = originalTags.filter((item:ITag) => values.includes(item.name));
        let new_items:ITag[] = items;
        
        setTags(new_items);
        changeTags(new_items);
    };


    return <>
        <FormControl className={classes.formControl}>
            <InputLabel id="tags-checkbox-label">
                {t("missing_nutrients")}
            </InputLabel>
            <Select
                labelId="tags-checkbox-label"
                MenuProps={MenuProps}
                multiple
                value={missingItems.map(a => a.name)}
                onChange={handleChange}
                input={<Input/>}
                renderValue={(selected) => (
                <div className={classes.chips}>
                    {(selected as string[]).map((value, index) => (
                        <Chip 
                            key={value} 
                            label={value} 
                            variant="outlined"
                            style={TagStyle(index)}
                        />
                    ))}
                </div>
                )}
            >
                { loading || !originalTags ?
                    <ProgressBox/>
                :
                    originalTags.map((tag:ITag, index:number) => (
                        <MenuItem key={index} value={tag.name}>
                            <Checkbox 
                                checked={missingItems.map(a => a.name).indexOf(tag.name) > -1} 
                                className={classes.color_black}
                            />
                            <ListItemText primary={tag.name} />
                        </MenuItem>
                    ))
                }
            </Select>
        </FormControl>
        { onSaveTags &&
            <Box className={classes.control_button}>
                <ControlButtons 
                    onSave={() => onSaveTags(tags)} 
                    containStyle={false}
                    isAdmin={true}
                    submitOn="save"
                />
            </Box>
        }
    </>
}


