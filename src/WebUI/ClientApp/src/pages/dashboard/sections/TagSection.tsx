import * as React from 'react';
import { useEffect, useState } from 'react';
import { Box, FormControlLabel, Grid, Paper, Switch } from '@material-ui/core';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';

import ITag, { emptyTag } from '../../../interfaces/tag/ITag';
import { setErrorStatus, setSuccessStatus } from '../../../store/status/StatusActions';
import { httpDeleteTag, httpGetTags, httpPostTag, httpPutTag } from '../../../services/TagService';
import ControlButtons from '../../../components/control_buttons/ControlButtons';
import ProgressBox from '../../../components/progress/ProgressBox';
import EditKeyValueDialog from '../../../components/dialog/EditKeyValueDialog';
import ConfirmDialog from '../../../components/dialog/ConfirmDialog';
import CheckedTextList from '../../../components/list/CheckedTextList';
import TagPanelSectionStyle from './TagSectionStyle';
import EditText from '../../../components/textfields/EditText';

export default () => {
    const { t } = useTranslation();
    const dispatch = useDispatch();
    const classes = TagPanelSectionStyle();
    const [loading, setLoading] = useState<boolean|undefined>(undefined); 
    const [newTag, setNewTag] = useState<ITag>(emptyTag); 
    const [currentTag, setCurrentTag] = useState<ITag>(emptyTag); 
    const [allTags, setAllTags] = useState<ITag[]|undefined>(undefined); 
    const [visableCreate, setVisableCreate] = useState(false);
    const [visableDelete, setVisableDelete] = useState(false);
    
    // Initialize
    useEffect(() => {
        if(loading === undefined)
        {
            setLoading(true);

            httpGetTags(
                (data:ITag[]) => {
                    setAllTags(data ? data : []);
                    setCurrentTag(data[0] ? data[0] : emptyTag);
                    setLoading(false);
                },
                (message:string) => {
                    dispatch(setErrorStatus(message))
                    setLoading(false);
                }
            );
        }
    });
    
    const onCreateTag = () => 
    {
        if(!allTags) return;
        setLoading(true);

        httpPostTag(newTag, 
            (id:number) => {
                let value = { ...newTag, id:id };
                let values = [...allTags, value];
                
                setAllTags(values);
                setCurrentTag(value);
                setNewTag(emptyTag);

                setVisableCreate(false);
                setLoading(false);
                dispatch(setSuccessStatus("On Success"));
            },
            (message:string) => {
                setLoading(false);
                dispatch(setErrorStatus(message));
            }
        );
    }

    const onEditTag = () => 
    {
        if(!allTags) return;
        setLoading(true);

        httpPutTag(String(currentTag.id), currentTag, 
            (id:number) => {
                const values = allTags.map(
                    item => (item.id == id) ? currentTag : item
                );

                setAllTags(values);

                setLoading(false);
                dispatch(setSuccessStatus("On Success"));
            },
            (message:string) => {
                setLoading(false);
                dispatch(setErrorStatus(message))
            }
        );  
    }

    const onDeleteTag = () => 
    {
        if(!allTags) return;
        setLoading(true);

        httpDeleteTag(String(currentTag.id), 
            (id:number) => {
                const values = allTags.filter(item =>  item.id !== id);

                setAllTags(values);
                setCurrentTag(values[0]);

                setVisableDelete(false);
                setLoading(false);
                dispatch(setSuccessStatus("On Success"));
            },
            (message:string) => {
                setLoading(false);
                dispatch(setErrorStatus(message));
            }
        );
    }
    
    return <>
        <Box className={classes.panel}>
            <Grid container spacing={3} className={classes.container}>
                <Grid item xs={12} md={4} lg={3}>
                    <Paper className={classes.paper}>
                        <ControlButtons
                            onCreate={() => setVisableCreate(true)}
                            isAdmin={true}
                        />
                        
                        {(allTags === undefined || loading) ?
                            <ProgressBox/>
                            :   
                            <CheckedTextList
                                labelName='name'
                                labelHint='scientificName'
                                data={allTags} 
                                checked={[currentTag]} 
                                onCheck={(item:any) => () => setCurrentTag(item)}
                            />
                        }
                    </Paper>
                </Grid>
                {(allTags && allTags.length > 0) && 
                    <Grid item xs={12} md={8} lg={9}>
                        <form onSubmit={onEditTag}>
                            <Paper className={classes.paper}>
                                <FormControlLabel
                                    labelPlacement="start"
                                    label={t("hasResource")}
                                    control={
                                        <Switch 
                                            color="primary"
                                            checked={currentTag.hasResource}
                                            onChange={(value:any) => setCurrentTag({...currentTag, hasResource:!currentTag.hasResource})} 
                                        />
                                    }
                                />
                                <EditText 
                                    label={t("name")} 
                                    value={currentTag.name ? currentTag.name : ""} 
                                    setValue={(value:string) => setCurrentTag({...currentTag, name:value})}
                                />
                                <EditText 
                                    label={t("scientificName")} 
                                    value={currentTag.scientificName ? currentTag.scientificName : ""} 
                                    setValue={(value:string) => setCurrentTag({...currentTag, scientificName:value})}
                                />
                                <EditText 
                                    label={t("description")} 
                                    value={currentTag.description ? currentTag.description : ""} 
                                    setValue={(value:string) => setCurrentTag({...currentTag, description:value})}
                                    multilines
                                />
                                <Box className={classes.control_button}>
                                    <ControlButtons
                                        onDelete={() => setVisableDelete(true)}
                                        isAdmin={true}
                                        containStyle={false}
                                        style={{margin:"5px"}}
                                    />
                                    <ControlButtons 
                                        onSave={onEditTag} 
                                        isAdmin={true}
                                        submitOn="save"
                                        containStyle={false}
                                        style={{margin:"5px"}}
                                    />
                                </Box>
                            </Paper>
                        </form>
                    </Grid>
                }
            </Grid>
        </Box>
        <EditKeyValueDialog 
            title={t("create")}
            data={{name: newTag.name, scientificName:newTag.scientificName, hasResource:newTag.hasResource}}
            setData={setNewTag}
            open={visableCreate} 
            setOpen={setVisableCreate} 
            isLoading={loading ? true : false}
            onConfirm={onCreateTag}
        />
        <ConfirmDialog 
            open={visableDelete} 
            setOpen={setVisableDelete} 
            isLoading={loading ? true : false}
            onConfirm={onDeleteTag}
        />
    </>
      
}
