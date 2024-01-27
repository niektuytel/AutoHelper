import { Dialog, DialogActions, DialogContent, DialogTitle, TextField, Button, Select, MenuItem, FormControl, InputLabel, Typography, FormHelperText, Tooltip, IconButton, Grid, CircularProgress } from '@mui/material';
import { useEffect, useState } from 'react';
import { Controller, useForm } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { useDispatch } from 'react-redux';
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import { EnumValues } from 'enum-values';
import { CreateVehicleEventNotifierCommand, VehicleClient } from '../../../../app/web-api-client';
import { showOnSuccess } from '../../../../redux/slices/statusSnackbarSlice';
import { useParams } from 'react-router';

// custom imports

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

interface FormInput {
    whatsappOrEmail: string;
}

interface QuestionDialogProps {
    open: boolean;
    onClose: () => void;
}

export default ({ open, onClose }: QuestionDialogProps) => {
    const { license_plate } = useParams();
    const dispatch = useDispatch();
    const { t } = useTranslation();
    const { control, handleSubmit, formState: { errors } } = useForm<FormInput>({
        defaultValues: {
            whatsappOrEmail: ''
        }
    });

    const [loading, setLoading] = useState<boolean>(false);
    const vehicleClient = new VehicleClient(process.env.PUBLIC_URL);

    const createNotification = async (command: CreateVehicleEventNotifierCommand) => {
        setLoading(true);
        try {
            console.log(command);
            const response = await vehicleClient.createServiceEventNotifier(command);

            setLoading(false);
            return response;
        } catch (response: any) {
            setLoading(false);
            throw response;
        }
    }

    const onSubmit = async (data: FormInput) => {
        let { whatsappOrEmail } = data;
        var command = new CreateVehicleEventNotifierCommand({
            vehicleLicensePlate: license_plate || ''
        });

        // Check the type of whatsappOrEmail and set corresponding values
        if (isValidEmail(whatsappOrEmail)) {
            command.receiverEmailAddress = whatsappOrEmail;
        } else if (isValidPhoneNumber(whatsappOrEmail)) {
            command.receiverWhatsappNumber = whatsappOrEmail;
        } else {
            console.error("Invalid input for whatsappOrEmail");
            return;
        }
        
        var response = await createNotification(command);

        dispatch(showOnSuccess(t("NotificationDialog.Confirm")));
        onClose();
    }

    return (
        <Dialog open={open} onClose={() => onClose()}>
            <DialogTitle>
                {t("NotificationDialog.Title")}
            </DialogTitle>
            <DialogContent>
                <Typography variant="body2" paragraph>
                    {t("NotificationDialog.Description")}
                </Typography>
                <Grid container spacing={2}>
                    <Grid item xs={12}>
                        <Controller
                            name="whatsappOrEmail"
                            control={control}
                            defaultValue=""
                            rules={{
                                required: t("NotificationDialog.WhatsappOrEmail.Required"),
                                validate: value => isValidEmail(value) || isValidPhoneNumber(value) || t("NotificationDialog.WhatsappOrEmail.Invalid")
                            }}
                            render={({ field }) => (
                                <TextField
                                    fullWidth
                                    label={t("NotificationDialog.WhatsappOrEmail.Label")}
                                    {...field}
                                    error={!!errors.whatsappOrEmail}
                                    helperText={errors.whatsappOrEmail?.message}
                                />
                            )}
                        />
                    </Grid>
                </Grid>
            </DialogContent>
            <DialogActions>
                <Button onClick={() => onClose()} style={{ textTransform: 'capitalize' }}>
                    {t("Cancel")}
                </Button>
                <Button
                    variant="outlined"
                    onClick={handleSubmit(onSubmit)}
                    style={{ textTransform: 'capitalize' }}
                    disabled={loading}
                >
                    {loading ? <CircularProgress size={24} /> : t("Confirm")}
                </Button>
            </DialogActions>
        </Dialog>
    );
}
