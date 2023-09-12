import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Button } from '@mui/material';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';


interface IProps {
    confirmDeleteOpen: boolean;
    setConfirmDeleteOpen: (confirmDeleteOpen: boolean) => void;
}

export default ({ confirmDeleteOpen, setConfirmDeleteOpen }: IProps) => {
    const { t } = useTranslation();

    return (
        <Dialog
            open={confirmDeleteOpen}
            onClose={() => setConfirmDeleteOpen(false)}
        >
            <DialogTitle>{t("Confirm Deletion")}</DialogTitle>
            <DialogContent>
                <DialogContentText>
                    {t("Are you sure you want to delete this service?")}
                </DialogContentText>
            </DialogContent>
            <DialogActions>
                <Button onClick={() => setConfirmDeleteOpen(false)} color="primary">
                    {t("Cancel")}
                </Button>
                <Button 
                    onClick={() => {
                        // Add your delete logic here...
                        setConfirmDeleteOpen(false);
                    }} 
                    color="primary"
                >
                    {t("Confirm")}
                </Button>
            </DialogActions>
        </Dialog>
    );
}
