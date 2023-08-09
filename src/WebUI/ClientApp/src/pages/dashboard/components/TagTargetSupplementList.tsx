import * as React from 'react';
import { useState } from 'react';
import { Grid, Paper } from '@material-ui/core';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';

import ITag from '../../../interfaces/tag/ITag';
import { setErrorStatus, setSuccessStatus } from '../../../store/status/StatusActions';
import ControlButtons from '../../../components/control_buttons/ControlButtons';
import ProgressBox from '../../../components/progress/ProgressBox';
import EditKeyValueDialog from '../../../components/dialog/EditKeyValueDialog';
import ConfirmDialog from '../../../components/dialog/ConfirmDialog';
import CheckedTextList from '../../../components/list/CheckedTextList';
import ITagFilterTarget from '../../../interfaces/tag/ITagFilterTarget';
import ITagSupplement, { emptyTagSupplement } from '../../../interfaces/tag/ITagSupplement';
import { httpDeleteTagFilterTargetSupplement, httpPostTagFilterTargetSupplement, httpPutTagFilterTargetSupplement } from '../../../services/TagFilterService';
import TagSectionStyle from '../sections/TagSectionStyle';


interface IProps {
    tags: ITag[]|undefined;
    currentTarget: ITagFilterTarget;
    setCurrentTarget: (data: ITagFilterTarget) => void;
    targets: ITagFilterTarget[] | undefined;
    setTargets: (data: ITagFilterTarget[]) => void;
    currentTargetSupplement: ITagSupplement;
    setCurrentTargetSupplement: (data: ITagSupplement) => void;
}

export default ({
    tags,
    currentTarget,
    setCurrentTarget,
    targets,
    setTargets
}:IProps) => {
    const { t } = useTranslation();
    const dispatch = useDispatch();
    const classes = TagSectionStyle();
    const [currentSupplement, setCurrentSupplement] = useState<ITagSupplement>(emptyTagSupplement);
    const [newSupplement, setNewSupplement] = useState<ITagSupplement>(emptyTagSupplement); 

    const [loading, setLoading] = useState<boolean|undefined>(undefined); 
    const [visableCreate, setVisableCreate] = useState(false);
    const [visableEdit, setVisableEdit] = useState(false);
    const [visableDelete, setVisableDelete] = useState(false);

    const onCreateSupplement = () => 
    {
        if(!targets) return;
        setLoading(true);

        httpPostTagFilterTargetSupplement({ ...newSupplement, targetId:currentTarget.id}, 
            (id:number) => {
                let value = {...newSupplement, id:id};
                let newTarget = currentTarget;
                newTarget.supplements.push(value);

                const newTargets = targets.map(item => (item.id == newTarget.id) ? newTarget : item);

                setTargets(newTargets);
                setCurrentTarget(newTarget);
                setCurrentSupplement(value);
                setNewSupplement(emptyTagSupplement);

                dispatch(setSuccessStatus("On Success"));
                setVisableCreate(false);
                setLoading(false);
            },
            (message:string) => {
                dispatch(setErrorStatus(message))
                setLoading(false);
            }
        ); 
    }

    const onEditSupplement = () => 
    {
        if(!targets) return;
        setLoading(true);
        
        httpPutTagFilterTargetSupplement(currentSupplement.id, currentSupplement, 
            (id:number) => {
                let newTarget = currentTarget;
                newTarget.supplements = newTarget.supplements.map(item => (item.id === currentSupplement.id) ? currentSupplement : item);
                setCurrentTarget(newTarget);

                const newTargets = targets.map(item => (item.id === newTarget.id) ? newTarget : item);
                setTargets(newTargets);
                setCurrentSupplement(currentSupplement);

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

    const onDeleteSupplement = () => 
    {
        if(!targets) return;
        setLoading(true);

        httpDeleteTagFilterTargetSupplement(currentSupplement.id, 
            (id:number) => {
                let newTarget = currentTarget;
                newTarget.supplements = newTarget.supplements.filter(item => item.id !== id);
                setCurrentTarget(newTarget);

                const newTargets = targets.map(item => (item.id === newTarget.id) ? newTarget : item);
                setTargets(newTargets);
                if (newTarget.supplements.length > 0) 
                {
                    setCurrentSupplement(newTarget.supplements[newTarget.supplements.length-1]);
                }

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
        { currentTarget.id !== 0 &&
            <Grid item xs={12} md={8} lg={9}>
                <Paper className={classes.paper}>
                    <ControlButtons
                        onCreate={() => setVisableCreate(true)}
                        onEdit={() => setVisableEdit(true)}
                        onDelete={() => setVisableDelete(true)}
                        isAdmin={true}
                    />
                    {!currentTarget.supplements ? 
                        <ProgressBox/>
                    :
                        <CheckedTextList 
                            labelName="tag.name"
                            data={currentTarget.supplements} 
                            checked={[currentSupplement]} 
                            onCheck={(item:any) => () => setCurrentSupplement(item)}
                        />
                    }
                </Paper>
            </Grid>
        }
        <EditKeyValueDialog 
            title={t("create")}
            data={newSupplement}
            setData={setNewSupplement}
            open={visableCreate} 
            setOpen={setVisableCreate} 
            isLoading={loading ? true : false}
            onConfirm={onCreateSupplement}
            tags={tags}
        />
        <EditKeyValueDialog 
            title={t("edit")}
            data={currentSupplement}
            setData={setCurrentSupplement}
            open={visableEdit} 
            setOpen={setVisableEdit} 
            isLoading={loading ? true : false}
            onConfirm={onEditSupplement}
            tags={tags}
        />
        <ConfirmDialog 
            open={visableDelete} 
            setOpen={setVisableDelete} 
            isLoading={loading ? true : false}
            onConfirm={onDeleteSupplement}
        />
    </>
      
}
