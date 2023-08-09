import * as React from 'react';
import { Checkbox, List, ListItem, ListItemIcon } from '@material-ui/core';
import { useSelector } from 'react-redux';
import { colorOnIndex } from '../../../i18n/ColorValues';
import TagFilterListStyle from './TagFilterListStyle';
import ITagFilter from '../../../interfaces/tag/ITagFilter';
import TagFilterState from '../../../store/tag_filter/TagFilterState';

interface IProps {
    currentFilter: ITagFilter;
    setCurrentFilter: (item:ITagFilter) => void;
}

export default ({setCurrentFilter, currentFilter}:IProps) => {
    const classes = TagFilterListStyle();
    const { filters }:TagFilterState = useSelector((state:any) => state.tag_filters);

    return <>
        <List className={classes.root}>
            {filters && filters.map((item:ITagFilter, index:number) => 
                <ListItem 
                    key={`question-list-${index}`}
                    onClick={() => setCurrentFilter(item)}
                    title={item.title}
                    role={undefined} 
                    dense 
                    button 
                >
                    <ListItemIcon>
                        <Checkbox
                            edge="start"
                            tabIndex={-1}
                            disableRipple
                            checked={currentFilter ? item.id === currentFilter.id : false}
                            style={{color:colorOnIndex(index)}}
                        />
                    </ListItemIcon>
                    {item.title}
                </ListItem>
            )}
        </List>
    </>
}
