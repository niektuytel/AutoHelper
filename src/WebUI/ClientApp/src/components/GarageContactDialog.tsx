import { Dialog, DialogActions, DialogContent, DialogTitle, TextField, Button, Select, MenuItem, FormControl, InputLabel, Typography, FormHelperText, Tooltip, IconButton, Grid } from '@mui/material';
import { useEffect, useState } from 'react';
import { Controller, useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import { EnumValues } from 'enum-values';

// custom imports
import { ConversationType, GarageServiceType, MessageClient, SelectedService, SelectedServices } from '../app/web-api-client';
import { showOnSuccess } from '../redux/slices/statusSnackbarSlice';

const isValidEmail = (input: string): boolean => {
    // Simple regex for email validation
    const emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
    return emailRegex.test(input);
}

const isValidPhoneNumber = (input: string): boolean => {
    // Basic regex for phone number validation (can be adjusted based on specific needs)
    const phoneRegex = /^(\+?[1-9]\d{1,14}|[0-9]{9,10})$/;
    return phoneRegex.test(input);
}

function convertToEnumValue(type: string): number | undefined {
    return ConversationType[type as keyof typeof ConversationType];
}

interface FormInput {
    whatsappOrEmail: string;
    messageType: string;
    message: string;
}

interface QuestionDialogProps {
    requestQuote?: boolean;
    services: SelectedService[];
    open: boolean;
    onClose: () => void;
}

export default ({ requestQuote = false, services, open, onClose }: QuestionDialogProps) => {
    const dispatch = useDispatch();
    const { t } = useTranslation();
    const { control, handleSubmit, setValue, watch, formState: { errors } } = useForm<FormInput>({
        defaultValues: {
            whatsappOrEmail: '',
            messageType: '',
            message: '',
        }
    });

    const [loading, setLoading] = useState<boolean>(false);
    const conversationClient = new MessageClient(process.env.PUBLIC_URL);

    const startConversations = async (command: SelectedServices) => {
        setLoading(true);
        try {
            console.log(command);
            const response = await conversationClient.startConversations(command);

            setLoading(false);
            return response;
        } catch (response: any) {
            setLoading(false);
            throw response;
        }
    }

    const watchedMessageType = watch('messageType', '');

    useEffect(() => {
        if (watchedMessageType) {
            const sampleMessage = t(`ConversationType.${watchedMessageType}.SampleMessage`);
            setValue('message', sampleMessage);
        }
    }, [watchedMessageType, setValue, t]);

    const onSubmit = async (data: FormInput) => {
        let { whatsappOrEmail, messageType, message } = data;
        let senderPhoneNumber: string | undefined;
        let senderWhatsappNumber: string | undefined;
        let senderEmailAddress: string | undefined;

        // Check the type of whatsappOrEmail and set corresponding values
        if (isValidPhoneNumber(whatsappOrEmail)) {
            senderPhoneNumber = whatsappOrEmail;
            senderWhatsappNumber = whatsappOrEmail; // If you want to set both
        } else if (isValidEmail(whatsappOrEmail)) {
            senderEmailAddress = whatsappOrEmail;
        } else {
            console.error("Invalid input for whatsappOrEmail");
            return; // Exit early if invalid input
        }

        // Check if the request a quote is selected
        if (requestQuote) {
            const enumValue = ConversationType.RequestAQuote;

            var conversations = new SelectedServices({
                senderEmailAddress: senderEmailAddress,
                senderPhoneNumber: senderPhoneNumber,
                senderWhatsappNumber: senderWhatsappNumber,
                messageType: enumValue,
                messageContent: message,
                services: services
            });
            var response = await startConversations(conversations);
        }
        else {
            const enumValue = convertToEnumValue(messageType);
            if (enumValue === undefined) {
                console.error("Invalid messageType");
                return; // Exit early if invalid messageType
            }

            var conversations = new SelectedServices({
                senderEmailAddress: senderEmailAddress,
                senderPhoneNumber: senderPhoneNumber,
                senderWhatsappNumber: senderWhatsappNumber,
                messageType: enumValue,
                messageContent: message,
                services: services
            });
            var response = await startConversations(conversations);
        }

        if (services.length === 0) {
            dispatch(showOnSuccess(t("Conversation.Started")));
        } else {
            dispatch(showOnSuccess(t("Conversations.Started")));
        }

        onClose();
    }

    return (
        <Dialog open={open} onClose={onClose}>
            <DialogTitle>
                {requestQuote ? t("Request a quote") : t("Ask a Question")}{` ${t("by")}`}
                <Tooltip title={requestQuote ? t("Request a quote.Tooltip") : t("Ask a Question.Tooltip")}>
                    <IconButton size="small">
                        <InfoOutlinedIcon fontSize="inherit" />
                    </IconButton>
                </Tooltip><br/>
                [{[...new Set(services?.map(x => x.relatedGarageLookupName?.toLowerCase()))].join(' + ')}]
            </DialogTitle>
            <DialogContent>
                <Typography variant="body2" paragraph>
                    {requestQuote ? t("Request a quote.Description") : t("Ask a Question.Description")}
                </Typography>
                <Grid container spacing={2}>
                    <Grid item xs={6}>
                        <Controller
                            name="whatsappOrEmail"
                            control={control}
                            defaultValue=""
                            rules={{
                                required: t("Ask a Question.WhatsappOrEmail.Required"),
                                validate: value => isValidEmail(value) || isValidPhoneNumber(value) || t("Ask a Question.WhatsappOrEmail.Invalid")
                            }}
                            render={({ field }) => (
                                <TextField
                                    fullWidth
                                    label={t("Ask a Question.WhatsappOrEmail.Label")}
                                    {...field}
                                    error={!!errors.whatsappOrEmail}
                                    helperText={errors.whatsappOrEmail?.message}
                                    sx={requestQuote ? {} : { marginBottom: 2 }}
                                />
                            )}
                        />
                    </Grid>
                    <Grid item xs={6}>
                        <TextField
                            fullWidth
                            label="To: whatsapp/email"
                            value={[...new Set(services?.map(x => x.receiverWhatsAppNumberOrEmail))].join(' & ')}
                            disabled
                            sx={requestQuote ? {} : { marginBottom: 2 }}
                        />
                    </Grid>
                </Grid>
                {!requestQuote && 
                    <>
                        <Controller
                            name="messageType"
                            control={control}
                            rules={{
                                required: requestQuote ? undefined : t("Ask a Question.MessageType.Required")
                            }}
                            render={({ field }) => (
                                <FormControl fullWidth variant="outlined" sx={{ marginBottom: 2 }} error={!!errors.messageType}>
                                    <InputLabel htmlFor="select-title">{t("Ask a Question.MessageType.Label")}</InputLabel>
                                    <Select {...field} label={t("Ask a Question.MessageType.Label")}>
                                        {
                                            EnumValues.getNames(ConversationType).map((type) => (
                                                <MenuItem key={type} value={type}>{t(`ConversationType.${type}`)}</MenuItem>
                                            ))
                                        }
                                    </Select>
                                    {errors.messageType && <FormHelperText>{errors.messageType.message}</FormHelperText>}
                                </FormControl>
                            )}
                        />
                        <Controller
                            name="message"
                            control={control}
                            defaultValue=""
                            render={({ field }) => (
                                <TextField
                                    fullWidth
                                    multiline
                                    rows={4}
                                    label={t("Ask a Question.Message.Label")}
                                    {...field}
                                />
                            )}
                        />
                    </>
                }
            </DialogContent>
            <DialogActions>
                <Button onClick={onClose} style={{ textTransform: 'capitalize' }}>{t("Cancel")}</Button>
                <Button variant={"outlined"} onClick={handleSubmit(onSubmit)} style={{ textTransform: 'capitalize' }}>{t("Send")}</Button>
            </DialogActions>
        </Dialog>
    );
}
