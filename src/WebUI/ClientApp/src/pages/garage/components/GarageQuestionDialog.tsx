import { Dialog, DialogActions, DialogContent, DialogTitle, TextField, Button, Select, MenuItem, FormControl, InputLabel, Typography } from '@mui/material';
import { useEffect, useState } from 'react';

interface QuestionDialogProps {
    open: boolean;
    onClose: () => void;
    onSubmit: (data: { emailOrPhone: string; messageType: string; message: string; }) => void;
}

export default ({ open, onClose, onSubmit }: QuestionDialogProps) => {
    const [emailOrPhone, setEmailOrPhone] = useState<string>('');
    const [messageType, setMessageType] = useState<string>('prijs aanvraag');
    const [message, setMessage] = useState<string>('');

    const messageTemplates: { [key: string]: string } = {
        'prijs aanvraag': 'Vraag over prijs voor mijn voertuig...',
        'technische vraag': 'Technische vraag over mijn voertuig...',
        'anders': 'Mijn vraag...',
    };

    useEffect(() => {
        setMessage(messageTemplates[messageType]);
    }, [messageType]);

    const handleSend = () => {
        onSubmit({ emailOrPhone, messageType, message });
        onClose();
    }

    // TODO: use ConversationMessageType

    return (
        <Dialog open={open} onClose={onClose}>
            <DialogTitle>Ask a Question</DialogTitle>
            <DialogContent>
                <Typography variant="body2" paragraph>
                    We zullen de garage de vraag stellen met betrekking tot uw voertuig. Als hij reageert, houden we u op de hoogte via WhatsApp (/e-mail).
                </Typography>
                <TextField
                    fullWidth
                    label="Email or Phone Number"
                    value={emailOrPhone}
                    onChange={e => setEmailOrPhone(e.target.value)}
                    sx={{ marginBottom: 2 }}
                />
                <FormControl fullWidth sx={{ marginBottom: 2 }}>
                    <InputLabel>Vraagtype</InputLabel>
                    <Select
                        value={messageType}
                        onChange={e => setMessageType(e.target.value as string)}
                    >
                        <MenuItem value="prijs aanvraag">Prijs vraag</MenuItem>
                        <MenuItem value="technische vraag">Technische vraag</MenuItem>
                        <MenuItem value="anders">Anders</MenuItem>
                    </Select>
                </FormControl>
                <TextField
                    fullWidth
                    multiline
                    rows={4}
                    label="Message"
                    value={message}
                    onChange={e => setMessage(e.target.value)}
                />
            </DialogContent>
            <DialogActions>
                <Button onClick={onClose}>Cancel</Button>
                <Button onClick={handleSend}>Send</Button>
            </DialogActions>
        </Dialog>
    );
}
