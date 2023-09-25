import * as React from 'react';
import { Button, Paper, Box, Typography } from '@mui/material';
import Icon1 from '@mui/icons-material/Cloud';  // Just placeholder icons for demonstration
import Icon2 from '@mui/icons-material/Favorite';
import Icon3 from '@mui/icons-material/Send';
import { COLORS } from '../../../constants/colors';
import { useNavigate } from 'react-router';
import { useTranslation } from 'react-i18next';

interface Props {
    onButton1Click?: () => void;
    onButton2Click?: () => void;
    onButton3Click?: () => void;
}

export default ({
    onButton1Click,
    onButton2Click,
    onButton3Click,
}: Props) => {
    const { t } = useTranslation();
    const navigate = useNavigate();

    // Navigate function that appends a hash to the current URL
    const navigateWithHash = (hashValue: string) => {
        navigate(`${window.location.pathname}${hashValue}`);
    }

    return (
        <Box
            display="flex"
            justifyContent="center"
            alignItems="center"
            gap={2}
            sx={{ marginTop: '15px' }}
        >
            <Paper elevation={3}>
                <Button
                    variant="contained"
                    color="primary"
                    onClick={() => { onButton1Click?.(); navigateWithHash("#general"); }}
                    style={{ width: '100px', height: '100px', flexDirection: 'column' }}
                >
                    <Icon1 />
                    <Typography variant="body2">{t('Information')}</Typography>
                </Button>
            </Paper>

            <Paper elevation={3}>
                <Button
                    variant="contained"
                    color="primary"
                    onClick={() => { onButton2Click?.(); navigateWithHash("#service_log"); }}
                    style={{ width: '100px', height: '100px', flexDirection: 'column' }}
                >
                    <Icon2 />
                    <Typography variant="body2">{t('maintenance logs')}</Typography>
                </Button>
            </Paper>
        </Box>
    );
};
