import React, { Dispatch, SetStateAction } from 'react';
import { TextField, Box, InputAdornment, IconButton, Typography, Paper, Grid, List, ListItem, Divider, FormControl, InputLabel, Select, MenuItem } from "@mui/material";
import ClearIcon from '@mui/icons-material/Clear';
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import { useTranslation } from "react-i18next";
import { Controller, FieldErrors, FieldValues } from 'react-hook-form';
import { GarageSettings, LocationItem } from '../../../../app/web-api-client';
import { idealBanks, idealIcon } from '../../../../constants/banking';

interface BankingSectionProps {
    state: {
        isLoading: boolean,
        garageSettings: GarageSettings
    };
    control: any;
    errors: FieldErrors<FieldValues>;
}
export default (
    { state, control, errors }: BankingSectionProps
) => {
    const { t } = useTranslation();

    return (
        <>
            {/* Contact Information Header */}
            <Grid item xs={12} style={{ marginTop: "40px" }}>
                <Typography variant="h5">{t("Contact")}</Typography>
                <Divider />
            </Grid>
            <Grid item xs={12} sm={6}>
                <Controller
                    name="phoneNumber"  // Refactored name
                    control={control}
                    rules={{ required: t("Phone Number is required!") }}
                    defaultValue={state.garageSettings.phoneNumber}
                    render={({ field }) => (
                        <TextField
                            {...field}
                            fullWidth
                            label={t("Phone number")}
                            variant="outlined"
                            error={Boolean(errors.phoneNumber)}  // Refactored error check
                            helperText={errors.phoneNumber ? t(errors.phoneNumber.message as string) : undefined}
                        />
                    )}
                />
            </Grid>
            <Grid item xs={12} sm={6}>
                <Controller
                    name="whatsAppNumber"  // Refactored name
                    control={control}
                    defaultValue={state.garageSettings.whatsAppNumber}
                    render={({ field }) => (
                        <TextField
                            {...field}
                            fullWidth
                            label={t("Whatsapp phone number")}
                            variant="outlined"
                        />
                    )}
                />
            </Grid>
            <Grid item xs={12} sm={6}>
                <Controller
                    name="email"
                    control={control}
                    rules={{ required: t("Email is required!") }}
                    defaultValue={state.garageSettings.email}
                    render={({ field }) => (
                        <TextField
                            {...field}
                            fullWidth
                            label={t("Email")}
                            variant="outlined"
                            error={Boolean(errors.email)}
                            helperText={errors.email ? t(errors.email.message as string) : undefined}
                        />
                    )}
                />
            </Grid>
        </>
    );
}
