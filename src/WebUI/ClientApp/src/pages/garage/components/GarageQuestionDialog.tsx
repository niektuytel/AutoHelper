import { Dialog, DialogActions, DialogContent, DialogTitle, TextField, Button, Select, MenuItem, FormControl, InputLabel, Typography, FormHelperText, Tooltip, IconButton } from '@mui/material';
import { useEffect, useState } from 'react';
import { Controller, useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import { ConversationClient, ConversationType, GarageServiceType, StartConversationBody } from '../../../app/web-api-client';
import { showOnSuccess } from '../../../redux/slices/statusSnackbarSlice';
import { EnumValues } from 'enum-values';

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
    garageLookupId: string;
    garageWhatsAppNumberOrEmail: string;
    relatedServiceTypes: GarageServiceType[];
    licensePlate: string;
    longitude: string;
    latitude: string;
    open: boolean;
    onClose: () => void;
}

export default ({ garageLookupId, garageWhatsAppNumberOrEmail, relatedServiceTypes, licensePlate, longitude, latitude, open, onClose }: QuestionDialogProps) => {
    const dispatch = useDispatch();
    const { t } = useTranslation();
    const { control, handleSubmit, setValue, getValues, register, formState: { errors }, watch } = useForm<FormInput>();

    const [loading, setLoading] = useState<boolean>(false);
    const conversationClient = new ConversationClient(process.env.PUBLIC_URL);
    const startConversation = async (command: StartConversationBody) => {
        setLoading(true);
        try {
            console.log(command);
            const response = await conversationClient.startConversation(command);

            setLoading(false);
            return response;
        } catch (response: any) {
            setLoading(false);
            throw response;
        }
    }

    // Use watch to observe changes to messageType field
    const watchedMessageType = watch('messageType');
    useEffect(() => {
        if (watchedMessageType) {
            const watchedMessage = watch('message');
            if (!watchedMessage) {
                setValue('message', t(`ConversationType.${watchedMessageType}.SampleMessage`));
            }
        }
    }, [watchedMessageType, setValue]);

    const onSubmit = async (data: FormInput) => {
        let { whatsappOrEmail, messageType, message } = data;
        let vehiclePhoneNumber: string | undefined;
        let vehicleWhatsappNumber: string | undefined;
        let vehicleEmailAddress: string | undefined;

        // Check the type of whatsappOrEmail and set corresponding values
        if (isValidPhoneNumber(whatsappOrEmail)) {
            vehiclePhoneNumber = whatsappOrEmail;
            vehicleWhatsappNumber = whatsappOrEmail; // If you want to set both
        } else if (isValidEmail(whatsappOrEmail)) {
            vehicleEmailAddress = whatsappOrEmail;
        } else {
            console.error("Invalid input for whatsappOrEmail");
            return; // Exit early if invalid input
        }

        const enumValue = convertToEnumValue(messageType);
        if (enumValue === undefined) {
            console.error("Invalid messageType");
            return; // Exit early if invalid messageType
        }

        var conversation = new StartConversationBody({
            relatedGarageLookupId: garageLookupId,
            relatedServiceTypes: relatedServiceTypes,
            vehicleLicensePlate: licensePlate,
            vehicleLongitude: longitude,
            vehicleLatitude: latitude,
            vehicleEmailAddress: vehicleEmailAddress,
            vehiclePhoneNumber: vehiclePhoneNumber,
            vehicleWhatsappNumber: vehicleWhatsappNumber,
            senderWhatsAppNumberOrEmail: whatsappOrEmail,
            receiverWhatsAppNumberOrEmail: garageWhatsAppNumberOrEmail,
            messageType: enumValue,
            messageContent: message
        });

        var response = await startConversation(conversation);
        dispatch(showOnSuccess(t("Conversation.Started")));

        onClose();
    }

    // TODO: use ConversationType

    return (
        <Dialog open={open} onClose={onClose}>
            <DialogTitle>
                {t("Ask a Question")}
                <Tooltip title={t("Ask a Question.Tooltip")}>
                    <IconButton size="small">
                        <InfoOutlinedIcon fontSize="inherit" />
                    </IconButton>
                </Tooltip>
            </DialogTitle>
            <DialogContent>
                <Typography variant="body2" paragraph>
                    {t("Ask a Question.Description")}
                </Typography>
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
                            sx={{ marginBottom: 2 }}
                        />
                    )}
                />
                <Controller
                    name="messageType"
                    control={control}
                    rules={{
                        required: t("Ask a Question.MessageType.Required")
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
            </DialogContent>
            <DialogActions>
                <Button onClick={onClose} style={{ textTransform: 'capitalize' }}>{t("Cancel")}</Button>
                <Button variant={"outlined"} onClick={handleSubmit(onSubmit)} style={{ textTransform: 'capitalize' }}>{t("Send")}</Button>
            </DialogActions>
        </Dialog>
    );
}
