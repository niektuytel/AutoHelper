import * as React from 'react';
import { Box, Grid, Paper } from '@material-ui/core';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';

import TagFilterSectionStyle from './TagFilterSectionStyle';
import { setErrorStatus, setSuccessStatus } from '../../../store/status/StatusActions';
import EditTextPaper from '../components/EditTextPaper';
import { httpDeleteTagFilterSituation, httpGetTagFilterSituations, httpPostTagFilterSituation, httpPutTagFilterSituation } from '../../../services/TagFilterService';
import TagMultiSelect from '../components/TagMultiSelect';
import ITagFilterSituation, { emptyTagFilterSituation } from '../../../interfaces/tag/ITagFilterSituation';
import ControlButtons from '../../../components/control_buttons/ControlButtons';
import TagSituationList from '../components/TagSituationList';
import EditKeyValueDialog from '../../../components/dialog/EditKeyValueDialog';
import ConfirmDialog from '../../../components/dialog/ConfirmDialog';
import TagTargetDropDown from '../components/TagTargetDropDown';
import TagFilterDropDown from '../components/TagFilterDropDown';
import EditText from '../../../components/textfields/EditText';

export default () => {
    const { t } = useTranslation();
    const classes = TagFilterSectionStyle();
    const dispatch = useDispatch();
    
    const [currentSituation, setCurrentSituation] = useState<ITagFilterSituation>(emptyTagFilterSituation);
    const [newSituation, setNewSituation] = useState<ITagFilterSituation>(emptyTagFilterSituation);
    const [situations, setSituations] = useState<ITagFilterSituation[]>([]);
    
    const [loading, setLoading] = useState<boolean|undefined>(undefined);
    const [visableCreate, setVisableCreate] = useState(false);
    const [visableDelete, setVisableDelete] = useState(false);

    // initialize data
    useEffect(() => {
        if(loading === undefined)
        {
            setLoading(true);
            httpGetTagFilterSituations(
                (data:ITagFilterSituation[]) => {
                    setLoading(false);
                    setSituations(data);
                },
                (message:string) => {
                    setLoading(false);
                    dispatch(setErrorStatus(message));
                }
            )
        }
        else if(situations.length > 0 && currentSituation.id === 0)
        {
            setCurrentSituation(situations[0])
        }
    });

    const onCreateSituation = () => {
        if(!situations) return;
        setLoading(true);

        httpPostTagFilterSituation(newSituation, 
            (id:number) => {
                let item = {...newSituation, id:id};
                let items = [...situations, item];
                
                setSituations(items);
                setCurrentSituation(item);
                setNewSituation(emptyTagFilterSituation);

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

    const onUpdateSituation = () => {
        if(!situations) return;
        setLoading(true);

        httpPutTagFilterSituation(currentSituation.id, currentSituation, 
            (id:number) => {
                setCurrentSituation(currentSituation);
                
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

    const onDeleteSituation = () => {
        if(!situations) return;
        setLoading(true);

        httpDeleteTagFilterSituation(currentSituation, 
            (id:number) => {
                let items = situations.filter((item) => item.id !== id);
                
                setSituations(items);
                setCurrentSituation(emptyTagFilterSituation);

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

    return <>
        <form onSubmit={onUpdateSituation}>
            <Grid container spacing={3} style={{ justifyContent:"center" }}>
                <Grid item xs={12} md={4} lg={3}>
                    <Paper className={classes.list_paper}>
                        <ControlButtons 
                            onCreate={() => setVisableCreate(true)}
                            onDelete={() => setVisableDelete(true)}
                            isAdmin={true}
                        />
                        <TagSituationList 
                            situations={situations} 
                            currentSituation={currentSituation} 
                            setCurrentSituation={setCurrentSituation} 
                        />
                    </Paper>
                </Grid>
                <Grid item xs={12} md={8} lg={9}>
                    <Paper className={classes.paper}>
                        <EditText 
                            label={"Situatie"} 
                            value={currentSituation.text} 
                            setValue={(value:any) => setCurrentSituation({...currentSituation, text:value})}
                        />
                        <TagTargetDropDown 
                            targetId={currentSituation.filterTargetId} 
                            setTargetId={(value:any) => setCurrentSituation({...currentSituation, filterTargetId:value})}
                        />
                        <TagFilterDropDown 
                            filterId={currentSituation.filterId} 
                            setFilterId={(value:any) => setCurrentSituation({...currentSituation, filterId:value})}
                        />
                        <TagMultiSelect 
                            missingItems={currentSituation.missingItems}
                            changeTags={(tags) => setCurrentSituation({...currentSituation, missingItems: tags})}
                        />
                        <Box className={classes.control_button}>
                            <ControlButtons 
                                onSave={onUpdateSituation} 
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
            data={newSituation}
            setData={setNewSituation}
            open={visableCreate} 
            setOpen={setVisableCreate} 
            isLoading={loading ? true : false}
            onConfirm={onCreateSituation}
            ignoredKeys={["nextFilterId"]}
        />
        <ConfirmDialog 
            open={visableDelete} 
            setOpen={setVisableDelete} 
            isLoading={loading ? true : false}
            onConfirm={onDeleteSituation}
        />
    </>
      
}

