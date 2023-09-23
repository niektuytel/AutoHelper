import React, { Dispatch, SetStateAction } from 'react';
import { TextField, Box, InputAdornment, IconButton, Typography, Paper, Grid, List, ListItem, Divider, FormControl, InputLabel, Select, MenuItem } from "@mui/material";
import ClearIcon from '@mui/icons-material/Clear';
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import { useTranslation } from "react-i18next";
import { Controller, FieldErrors, FieldValues, useForm } from 'react-hook-form';
import { GarageItem, GarageLocationItem } from '../../../../app/web-api-client';
import { idealBanks, idealIcon } from '../../../../constants/banking';

interface BankingSectionProps {
    control: any;
    errors: FieldErrors<FieldValues>;
}
export default (
    { control, errors }: BankingSectionProps
) => {
    const { t } = useTranslation();


    return (
        <>
            <Grid item xs={12} style={{ marginTop: "40px" }}>
                <Typography variant="h5">{t("Contact")}</Typography>
                <Divider />
            </Grid>
            <Grid item xs={12} sm={6}>
                <Controller
                    name="phoneNumber"
                    control={control}
                    rules={{ required: t("What is your garage telephone number?") }}
                    defaultValue={""}
                    render={({ field }) => (
                        <TextField
                            {...field}
                            fullWidth
                            size="medium"
                            label={t("Telephone number")}
                            variant="outlined"
                            error={Boolean(errors.phoneNumber)}
                            helperText={errors.phoneNumber ? t(errors.phoneNumber.message as string) : undefined}
                        />
                    )}
                />
            </Grid>
            <Grid item xs={12} sm={6}>
                <Controller
                    name="whatsAppNumber"
                    control={control}
                    defaultValue={""}
                    render={({ field }) => (
                        <TextField
                            {...field}
                            fullWidth
                            size="medium"
                            label={t("Whatsapp number")}
                            variant="outlined"
                        />
                    )}
                />
            </Grid>
            <Grid item xs={12} sm={6}>
                <Controller
                    name="email"
                    control={control}
                    rules={{ required: t("What is your garage email?") }}
                    defaultValue={""}
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
