import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Box, Button } from '@mui/material';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import { COLORS } from '../../../../constants/colors';
import { CreateGarageServiceCommand } from '../../../../app/web-api-client';


interface IProps {
    items: CreateGarageServiceCommand[];
}

export default ({ items }: IProps) => {
    const { t } = useTranslation();
    
    return (
        <Box position="fixed" bottom={0} width="100%" bgcolor={COLORS.BORDER_GRAY} zIndex={999}>
            <Box display="flex" style={{ whiteSpace: 'nowrap', overflowX:"auto" }}>
                {items.map(item => (
                    <Box mx={2} display="inline-block" key={item.type}>
                        {item.description} {/* Displaying item's name as an example */}
                    </Box>
                ))}
            </Box>
            <Button 
                variant="contained" 
                color="primary"
                onClick={() => {
                    // Add your confirm logic here...
                }}
            >
                {t("Confirm")}
            </Button>
        </Box>
    );
}
