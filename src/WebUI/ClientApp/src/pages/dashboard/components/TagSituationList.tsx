import * as React from 'react';
import { Checkbox, List, ListItem, ListItemIcon } from '@material-ui/core';
import { colorOnIndex } from '../../../i18n/ColorValues';
import TagFilterListStyle from './TagFilterListStyle';
import ITagFilterSituation from '../../../interfaces/tag/ITagFilterSituation';

interface IProps {
    situations: ITagFilterSituation[];
    currentSituation: ITagFilterSituation;
    setCurrentSituation: (item:ITagFilterSituation) => void;
}

export default ({situations, currentSituation, setCurrentSituation}:IProps) => {
    const classes = TagFilterListStyle();

    return <>
        <List className={classes.root}>
            {situations && situations.map((item:ITagFilterSituation, index:number) => 
                <ListItem 
                    key={`question-list-${index}`}
                    onClick={() => setCurrentSituation(item)}
                    title={item.text}
                    role={undefined} 
                    dense 
                    button 
                >
                    <ListItemIcon>
                        <Checkbox
                            edge="start"
                            tabIndex={-1}
                            disableRipple
                            checked={currentSituation ? item.id === currentSituation.id : false}
                            style={{color:colorOnIndex(index)}}
                        />
                    </ListItemIcon>
                    {item.text}
                </ListItem>
            )}
        </List>
    </>
}
