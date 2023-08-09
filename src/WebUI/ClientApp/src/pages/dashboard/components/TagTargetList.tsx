import * as React from 'react';
import { useEffect, useState } from 'react';
import { Box, Grid, Paper } from '@material-ui/core';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';

import ITag, { emptyTag } from '../../../interfaces/tag/ITag';
import { setErrorStatus, setSuccessStatus } from '../../../store/status/StatusActions';
import { httpDeleteTagFilterTarget, httpGetTagFilterTargets, httpPostTagFilterTarget, httpPutTagFilterTarget } from '../../../services/TagFilterService';
import ControlButtons from '../../../components/control_buttons/ControlButtons';
import ProgressBox from '../../../components/progress/ProgressBox';
import EditKeyValueDialog from '../../../components/dialog/EditKeyValueDialog';
import ConfirmDialog from '../../../components/dialog/ConfirmDialog';
import CheckedTextList from '../../../components/list/CheckedTextList';
import TagPanelSectionStyle from '../sections/TagSectionStyle';
import ITagFilterTarget, { emptyFilterTarget } from '../../../interfaces/tag/ITagFilterTarget';

interface IProps {
    currentTarget: ITagFilterTarget;
    setCurrentTarget: (data: ITagFilterTarget) => void;
    targets: ITagFilterTarget[] | undefined;
    setTargets: (data: ITagFilterTarget[]) => void;
}

export default ({currentTarget, setCurrentTarget, targets, setTargets}:IProps) => {
    const { t } = useTranslation();
    const dispatch = useDispatch();
    const classes = TagPanelSectionStyle();
    const [loading, setLoading] = useState<boolean|undefined>(undefined); 
    const [newTarget, setNewTarget] = useState<ITagFilterTarget>(emptyFilterTarget); 
    const [visableCreate, setVisableCreate] = useState(false);
    const [visableEdit, setVisableEdit] = useState(false);
    const [visableDelete, setVisableDelete] = useState(false);
    
    // Initialize
    useEffect(() => {
        if(loading === undefined)
        {
            setLoading(true);

            httpGetTagFilterTargets(
                (data:ITagFilterTarget[]) => {
                    if(data.length > 0)
                    {
                        setCurrentTarget(data[0]);
                    }

                    setTargets(data);
                    setLoading(false);
                },
                (message:string) => {
                    dispatch(setErrorStatus(message))
                    setLoading(false);
                }
            );
        }
    });

    const onCreateTarget = () => 
    {
        if(!targets) return;
        setLoading(true);

        httpPostTagFilterTarget(newTarget,
            (id:number) => {
                var item = { ...newTarget, id:id };
                var items = [ ...targets, item ];

                setTargets(items);
                setCurrentTarget(item);
                setNewTarget(emptyFilterTarget);

                dispatch(setSuccessStatus("On Success"));
                setVisableCreate(false);
                setLoading(false);
            }, 
            (message:string) => {
                dispatch(setErrorStatus(message));
                setLoading(false);
            }
        );
    }

    const onEditTarget = () => 
    {
        if(!targets) return;
        setLoading(true);

        httpPutTagFilterTarget(currentTarget.id, currentTarget, 
            (id:number) => {
                const values = targets.map(
                    item => (item.id == id) ? currentTarget : item
                );

                setTargets(values);
                setCurrentTarget(values[0]);

                dispatch(setSuccessStatus("On Success"));
                setVisableEdit(false);
                setLoading(false);
            },
            (message:string) => {
                dispatch(setErrorStatus(message))
                setLoading(false);
            }
        );  
    }

    const onDeleteTarget = () => 
    {
        if(!targets) return;
        setLoading(true);

        httpDeleteTagFilterTarget(currentTarget.id, 
            (id:number) => {
                const values = targets.filter(item => item.id !== id);

                setTargets(values);
                setCurrentTarget(values[0]);

                dispatch(setSuccessStatus("On Success"));
                setVisableDelete(false);
                setLoading(false);
            },
            (message:string) => {
                dispatch(setErrorStatus(message))
                setLoading(false);
            }
        );
    }
    
    return <>
        <Grid item xs={12} md={4} lg={3}>
            <Paper className={classes.paper}>
                <ControlButtons
                    onCreate={() => setVisableCreate(true)}
                    onEdit={() => setVisableEdit(true)}
                    onDelete={() => setVisableDelete(true)}
                    isAdmin={true}
                />
                {!targets ?
                    <ProgressBox/>
                    :   
                    <CheckedTextList 
                        labelName="title"
                        data={targets} 
                        checked={[currentTarget]} 
                        onCheck={(item:any) => () => setCurrentTarget(item)}
                    />
                }
            </Paper>
        </Grid>
        <EditKeyValueDialog 
            title={t("create")}
            data={newTarget}
            setData={setNewTarget}
            open={visableCreate} 
            setOpen={setVisableCreate} 
            isLoading={loading ? true : false}
            onConfirm={onCreateTarget}
        />
        <EditKeyValueDialog 
            title={t("edit")}
            data={currentTarget}
            setData={setCurrentTarget}
            open={visableEdit} 
            setOpen={setVisableEdit} 
            isLoading={loading ? true : false}
            onConfirm={onEditTarget}
        />
        <ConfirmDialog 
            open={visableDelete} 
            setOpen={setVisableDelete} 
            isLoading={loading ? true : false}
            onConfirm={onDeleteTarget}
        />
    </>
      
}
