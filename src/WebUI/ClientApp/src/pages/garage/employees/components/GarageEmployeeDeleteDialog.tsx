import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Button, CircularProgress } from '@mui/material';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';


interface IProps {
    service: any;
    confirmDeleteOpen: boolean;
    setConfirmDeleteOpen: (confirmDeleteOpen: boolean) => void;
    deleteService: (data: any) => void;
    loading: boolean;
}

export default ({ service, confirmDeleteOpen, setConfirmDeleteOpen, deleteService, loading }: IProps) => {
    const { t } = useTranslation();

    return (
        <Dialog
            open={confirmDeleteOpen}
            onClose={() => setConfirmDeleteOpen(false)}
        >
            <DialogTitle>
                {t("Confirm Deletion")}
            </DialogTitle>
            <DialogContent>
                <DialogContentText>
                    {t("Are you sure you want to delete this service?")}
                </DialogContentText>
            </DialogContent>
            <DialogActions>
                <Button onClick={() => setConfirmDeleteOpen(false)} color="primary">
                    {t("Cancel")}
                </Button>
                {loading ?
                    <Button variant="contained" disabled style={{ color: 'white' }}>
                        <CircularProgress size={24} color="inherit" />
                    </Button>
                    :
                    <Button onClick={() => deleteService(service)} type="submit" variant="contained" color="primary">
                        {t("Confirm")}
                    </Button>
                }
            </DialogActions>
        </Dialog>
    );
}
