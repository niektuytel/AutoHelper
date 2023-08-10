import React, { ReactNode, useState } from 'react';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import Typography from '@mui/material/Typography';
import DialogTitle from '@mui/material/DialogTitle';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import Grid from '@mui/material/Grid';
import Switch from '@mui/material/Switch';
import FormControlLabel from '@mui/material/FormControlLabel';
import { styled, Theme } from '@mui/material/styles';
import IconButton from '@mui/material/IconButton';
import PhotoCamera from '@mui/icons-material/PhotoCamera';
import { useTranslation } from 'react-i18next';
import EditText from '../textfields/EditText';
import TagDropDown from '../tags/TagDropDown';
import ITag from '../../interfaces/tag/ITag';
//import TagTargetDropDown from '../../pages/dashboard/components/TagTargetDropDown';
//import TagFilterDropDown from '../../pages/dashboard/components/TagFilterDropDown';
import INoteOnKey from '../../interfaces/INoteOnKey';
import DialogDynamicInput from './components/DialogDynamicInput';
import IProductSpecLine from '../../interfaces/product/IProductSpecLine';

const randomNumberInRange = (min:number, max:number) => {
    // 👇️ get number between min (inclusive) and max (inclusive)
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

interface IProps {
    title:string;
    data: any;
    setData: (value:any) => void;
    open:boolean;
    setOpen: (value:boolean) => void;
    isLoading: boolean;
    onConfirm: () => void;

    // specific actions
    disableForm?: boolean;
    tags?: ITag[];
    specifications?: IProductSpecLine[];
    onAttachFile?: (file:any) => void;
    multilines?: string[];
    ignoredKeys?: string[];
    noteOnKeys?: INoteOnKey[]
}
  
export default ({
    title, 
    data, 
    setData, 
    open, 
    setOpen, 
    isLoading, 
    onConfirm, 
    
    disableForm,
    tags, 
    specifications,
    onAttachFile, 
    multilines, 
    ignoredKeys, 
    noteOnKeys

}:IProps) => {
    const { t } = useTranslation();
    const [randint, setRandint] = useState(randomNumberInRange(1, 1000000));

    const handleClose = () => {
        setOpen(false);
    };

    return <>
        <Dialog 
            key={`dialog-${randint}`} 
            open={open}
            onClose={handleClose} 
            aria-labelledby={`customized-dialog-${title}`} 
        >
            <DialogTitle id={`customized-dialog-${title}`}>
                {title}
            </DialogTitle>
            {disableForm ? 
                <>
                    <DialogDynamicInput 
                        title={title} 
                        data={data} 
                        setData={setData}
                        tags={tags}
                        specifications={specifications}
                        onAttachFile={onAttachFile}
                        multilines={multilines}
                        ignoredKeys={ignoredKeys}
                        noteOnKeys={noteOnKeys}
                    />
                    <DialogActions>
                        <Button disabled={isLoading} onClick={() => onConfirm()} color="primary">
                            { isLoading ? t("loading") : t("save")}
                        </Button>
                    </DialogActions>
                </>
                :
                <form onSubmit={onConfirm}>
                    <DialogDynamicInput 
                        title={title} 
                        data={data} 
                        setData={setData}
                        tags={tags}
                        specifications={specifications}
                        onAttachFile={onAttachFile}
                        multilines={multilines}
                        ignoredKeys={ignoredKeys}
                        noteOnKeys={noteOnKeys}
                    />
                    <DialogActions>
                        <Button type={"submit"} disabled={isLoading} color="primary">
                            { isLoading ? t("loading") : t("save")}
                        </Button>
                    </DialogActions>
                </form>
            }
        </Dialog>
    </>
}