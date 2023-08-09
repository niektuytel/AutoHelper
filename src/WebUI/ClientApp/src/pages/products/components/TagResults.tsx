import React from "react";
import { Box, Chip } from "@material-ui/core";
import { makeStyles } from "@material-ui/styles";
import { colorOnIndex } from "../../../i18n/ColorValues";
import PersonalFilterState from "../../../store/tag_filtering/TagFilteringState";
import { useSelector } from "react-redux";
import ITag from "../../../interfaces/tag/ITag";

const TagResultsStyle = makeStyles(() => ({
    chip: {
        display: 'flex',
        justifyContent: 'center',
        flexWrap: 'wrap',
        '& > *': {
          margin: 0,
        }
    }
}))

export default () => {
    const classes = TagResultsStyle();
    const { usedSituations }: PersonalFilterState = useSelector((state:any) => state.tag_filtering);

    var items: { [k: string]: number } = {};
    Object.entries(usedSituations).forEach(([key, value]) => 
        value.missingItems.forEach((tag:ITag, index:number) =>  
        {
            if(!items[tag.name])
            {
                items[tag.name] = 1;
            }
            else
            {
                items[tag.name] = items[tag.name] + 1;
            }
        }
    ));

    return <Box style={{margin:"5px"}}>
        <div className={classes.chip}>
            {Object.entries(items)
                .sort((a, b) => b[1] - a[1])
                .map(([key, value], index:number) => 
                    <Chip 
                        key={`tag-${key}`}
                        label={value > 1 ? `${value}X ${key}` : key}
                        variant="outlined"
                        size="small" 
                        style={{ 
                            borderColor:colorOnIndex(index), 
                            color:"#000", 
                            borderRadius: "5px",
                            margin:"1px"
                        }} 
                    />
                )}
        </div>
    </Box>
}