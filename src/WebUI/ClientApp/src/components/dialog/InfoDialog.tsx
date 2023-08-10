import React from 'react';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import useMediaQuery from '@mui/material/useMediaQuery';
import { useTheme } from '@mui/material/styles';
import { useTranslation } from 'react-i18next';
import { useNavigate } from "react-router-dom";

interface IProps {
    tag: string;
    title?: string;
    content?: string;
    hasResource?: boolean;
    open: boolean;
    setOpen: (value: boolean) => void;
}

export default ({ tag, title, content, hasResource, open, setOpen }: IProps) => {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const theme = useTheme();
    const fullScreen = useMediaQuery(theme.breakpoints.down('xs'));

    if (!content) return <></>;
    return (
        <Dialog
            fullScreen={fullScreen}
            open={open}
            onClose={() => setOpen(false)}
            onClick={(event: any) => {
                event.stopPropagation();
                event.preventDefault();
            }}
            aria-labelledby="responsive-dialog-title"
        >
            {title &&
                <DialogTitle id="responsive-dialog-title">
                    {title}
                </DialogTitle>
            }
            <DialogContent>
                <DialogContentText sx={{ color: "black", whiteSpace: 'pre-line' }}>
                    {content}
                </DialogContentText>
            </DialogContent>
            <DialogActions>
                {hasResource &&
                    <Button onClick={() => navigate(`/info/${tag}`)}>
                        {t("more_information")}
                    </Button>
                }
                <Button variant="outlined" autoFocus onClick={(event: any) => {
                    setOpen(false);
                    event.stopPropagation();
                    event.preventDefault();
                }
                }>
                    {t("close")}
                </Button>
            </DialogActions>
        </Dialog>
    );
}
