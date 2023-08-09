import * as React from 'react';
import { Box, Grid, Paper } from '@material-ui/core';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch, useSelector } from 'react-redux';

import { setErrorStatus, setSuccessStatus } from '../../../store/status/StatusActions';
import ControlButtons from '../../../components/control_buttons/ControlButtons';
import EditKeyValueDialog from '../../../components/dialog/EditKeyValueDialog';
import ConfirmDialog from '../../../components/dialog/ConfirmDialog';
import { httpDeleteTagFilter, httpPostTagFilter, httpPutTagFilter } from '../../../services/TagFilterService';
import ITagFilter, { emptyTagFilter } from '../../../interfaces/tag/ITagFilter';
import TagFilterState from '../../../store/tag_filter/TagFilterState';
import TagFilterList from '../components/TagFilterList';
import EditTextPaper from '../components/EditTextPaper';
import TagTargetDropDown from '../components/TagTargetDropDown';
import TagFilterSectionStyle from './TagFilterSectionStyle';
import { getFilters, setFilters } from '../../../store/tag_filter/TagFilterActions';

export default () => {
    const { t } = useTranslation();
    const dispatch = useDispatch();
    const classes = TagFilterSectionStyle();
    const { isLoading, filters }:TagFilterState = useSelector((state:any) => state.tag_filters);

    const [currentFilter, setCurrentFilter] = useState<ITagFilter>(emptyTagFilter);
    const [newFilter, setNewFilter] = useState<ITagFilter>(emptyTagFilter);
    const [loading, setLoading] = useState<boolean>(false);
    const [visableCreate, setVisableCreate] = useState(false);
    const [visableDelete, setVisableDelete] = useState(false);

    useEffect(() => {
        if(!isLoading)
        {
            if(filters === undefined)
            {
                dispatch(getFilters());
            }
            else if(filters.length > 0 && currentFilter.id === 0)
            {
                setCurrentFilter(filters[0]);
            }
        }
    });

    const onCreateFilter = () => {
        if(!filters) return;
        setLoading(true);

        httpPostTagFilter(newFilter, 
            (id:number) => {
                let item = {...newFilter, id:id};
                let items = [...filters, item];
                
                dispatch(setFilters(items));
                setCurrentFilter(item);
                setNewFilter(emptyTagFilter);

                setLoading(false);
                setVisableCreate(false);
                dispatch(setSuccessStatus("On success"));
            }, 
            (message:string) => {
                setLoading(false);
                dispatch(setErrorStatus(message));
            }
        );
    }

    const onUpdateFilter = () => {
        if(!filters) return;
        setLoading(true);

        httpPutTagFilter(currentFilter.id, currentFilter, 
            (id:number) => {
                updateFilter(currentFilter);
                
                setLoading(false);
                setVisableCreate(false);
                dispatch(setSuccessStatus("On success"));
            }, 
            (message:string) => {
                setLoading(false);
                dispatch(setErrorStatus(message));
            }
        );
    }

    const onDeleteFilter = () => {
        if(!filters) return;
        setLoading(true);

        httpDeleteTagFilter(currentFilter.id, 
            (id:number) => {
                let items = filters.filter((item) => item.id !== id);
                
                dispatch(setFilters(items));
                setCurrentFilter(emptyTagFilter);

                setLoading(false);
                setVisableDelete(false);
                dispatch(setSuccessStatus("On success"));
            }, 
            (message:string) => {
                setLoading(false);
                dispatch(setErrorStatus(message));
            }
        );
    }

    const updateFilter = (filter: ITagFilter) => {
        if(!filters) return;
        let items = filters.map(item => item.id === filter.id ? filter : item);

        setCurrentFilter(filter);
        dispatch(setFilters(items));
    }

    return <>
        <form onSubmit={onUpdateFilter}>
            <Grid container spacing={3} style={{ justifyContent:"center" }}>
                <Grid item xs={12} md={4} lg={3}>
                    <Paper className={classes.list_paper}>
                        <ControlButtons 
                            onCreate={() => setVisableCreate(true)}
                            onDelete={() => setVisableDelete(true)}
                            isAdmin={true}
                        />
                        <TagFilterList 
                            currentFilter={currentFilter} 
                            setCurrentFilter={setCurrentFilter}
                        />
                    </Paper>
                </Grid>
                <Grid item xs={12} md={8} lg={9}>
                    <EditTextPaper
                        label={"Filter"}
                        value={currentFilter.title}
                        setValue={(value:any) => updateFilter({...currentFilter, title:value})}
                        onSave={onUpdateFilter}
                        isAdmin={true}
                    />
                    <Paper className={classes.paper}>
                        <TagTargetDropDown 
                            targetId={currentFilter.filterTargetId} 
                            setTargetId={(value:any) => updateFilter({...currentFilter, filterTargetId:value})}
                        />
                        <Box className={classes.control_button}>
                            <ControlButtons 
                                onSave={onUpdateFilter} 
                                isAdmin={true}
                                containStyle={false}
                                submitOn="save"
                            />
                        </Box>
                    </Paper>
                </Grid>
            </Grid>
        </form>
        <EditKeyValueDialog 
            title={t("create")}
            data={newFilter}
            setData={setNewFilter}
            open={visableCreate} 
            setOpen={setVisableCreate} 
            isLoading={loading}
            onConfirm={onCreateFilter}
        />
        <ConfirmDialog 
            open={visableDelete} 
            setOpen={setVisableDelete} 
            isLoading={loading}
            onConfirm={onDeleteFilter}
        />
    </>
      
}
