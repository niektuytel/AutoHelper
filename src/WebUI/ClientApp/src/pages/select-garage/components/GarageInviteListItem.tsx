
import { Box, Button, Chip, Paper, TextField, Typography } from '@mui/material';
import React, { useState } from 'react';
import { GarageLookupDto } from '../../../app/web-api-client';
import EuroSymbolIcon from '@mui/icons-material/EuroSymbol';
import PlaceIcon from '@mui/icons-material/Place';
import PublishedWithChangesIcon from '@mui/icons-material/PublishedWithChanges';
import ModeOfTravelIcon from '@mui/icons-material/ModeOfTravel';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import { DAYSINWEEKSHORT } from '../../../constants/days';
import { useTranslation } from 'react-i18next';
import ImageLogo from '../../../components/logo/ImageLogo';

interface IProps {
}

export default ({ }: IProps) => {
    const { t } = useTranslation();

    const [email, setEmail] = useState<string>('');
    const [whatsAppNumber, setWhatsAppNumber] = useState<string>('');

    const handleSendMessage = () => {
        // Logic to send message
        console.log(`Email: ${email}, WhatsApp Number: ${whatsAppNumber}`);
    };

    return <>
        <Paper
            variant="outlined"
            sx={{ mb: 1, p:1 }} 
        >
            <Box>
                <Box
                    style={{
                        display: "flex",
                        flexDirection: "column",
                        alignItems: "flex-start"
                    }}
                >
                    <Typography variant="h6">
                        Invite Garage
                    </Typography>

                    <Box display="flex" flexDirection="row" gap={2} sx={{ width: "-webkit-fill-available"}}>
                        <TextField
                            label="Email"
                            variant="outlined"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            fullWidth
                            size="small"
                        />
                        <TextField
                            label="WhatsApp Number"
                            variant="outlined"
                            value={whatsAppNumber}
                            onChange={(e) => setWhatsAppNumber(e.target.value)}
                            fullWidth
                            size="small"
                        />
                    </Box>
                    <Box display="flex" justifyContent="flex-start" width="100%">
                        <Button
                            variant="contained"
                            color="primary"
                            onClick={handleSendMessage}
                            sx={{ mt: 1 }}
                        >
                            Stuur een uitnodeging
                        </Button>
                    </Box>
                </Box>
            </Box>
        </Paper>
    </>;

}

