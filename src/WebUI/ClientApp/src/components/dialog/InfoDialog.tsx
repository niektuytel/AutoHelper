import React from 'react';
import Button from '@material-ui/core/Button';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogContent from '@material-ui/core/DialogContent';
import DialogContentText from '@material-ui/core/DialogContentText';
import DialogTitle from '@material-ui/core/DialogTitle';
import useMediaQuery from '@material-ui/core/useMediaQuery';
import { useTheme } from '@material-ui/core/styles';
import { useTranslation } from 'react-i18next';
import { useHistory } from 'react-router';

interface IProps {
    tag:string;
    title?: string;
    content?: string;
    hasResource?: boolean;
    open: boolean;
    setOpen: (value:boolean) => void;
}


export default ({tag, title, content, hasResource, open, setOpen}:IProps) => {
    const { t } = useTranslation();
    const history = useHistory();
    const theme = useTheme();
    const fullScreen = useMediaQuery(theme.breakpoints.down('xs'));

    if(!content) return<></>;
    return <>
        <Dialog
            fullScreen={fullScreen}
            open={open}
            onClose={() => setOpen(false)}
            onClick={
                (event:any) => {
                    event.stopPropagation();
                    event.preventDefault();
                }
            }
            aria-labelledby="responsive-dialog-title"
        >
            {title && 
                <DialogTitle id="responsive-dialog-title">
                    {title}
                </DialogTitle>
            }
            <DialogContent>
                <DialogContentText style={{color:"black", whiteSpace: 'pre-line'}}>
                    {content}
                </DialogContentText>
            </DialogContent>
            <DialogActions>
                {hasResource && 
                    <Button onClick={() => history.push(`/info/${tag}`)}>
                        {t("more_information")}
                    </Button>
                }
                <Button variant="outlined" autoFocus onClick={
                    (event:any) => {
                        setOpen(false);
                        event.stopPropagation();
                        event.preventDefault();
                    }
                }
                >
                    {t("close")}
                </Button>
            </DialogActions>
        </Dialog>
    </>
}