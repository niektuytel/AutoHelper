import React from 'react';
import Button from '@material-ui/core/Button';
import Dialog from '@material-ui/core/Dialog';
import DialogActions from '@material-ui/core/DialogActions';
import DialogTitle from '@material-ui/core/DialogTitle';
import useMediaQuery from '@material-ui/core/useMediaQuery';
import { useTheme } from '@material-ui/core/styles';
import { useTranslation } from 'react-i18next';

interface IProps {
  open:boolean;
  setOpen: (value:boolean) => void;
  isLoading: boolean;
  onConfirm: () => void;
}

export default ({open, setOpen, isLoading, onConfirm}:IProps) => {
  const { t } = useTranslation();
  const theme = useTheme();
  const fullScreen = useMediaQuery(theme.breakpoints.down('sm'));

  const handleClose = () => {
    setOpen(false);
  };

  return (
    <div>
      <Dialog
        fullScreen={fullScreen}
        open={open}
        onClose={handleClose}
        aria-labelledby="responsive-dialog-title"
      >
        <DialogTitle id="responsive-dialog-title">{t("are_you_sure")}</DialogTitle>
        <DialogActions>
          <Button 
            disabled={isLoading} 
            onClick={handleClose} 
            color="primary"
          >
            {t("no")}
          </Button>
          <Button 
            onClick={onConfirm}
            disabled={isLoading}
            color="primary" 
          >
            {isLoading ? t("loading") : t("yes")}
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}