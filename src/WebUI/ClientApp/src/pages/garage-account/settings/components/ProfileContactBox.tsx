import React, { Dispatch, SetStateAction } from 'react';
import { TextField, Box, InputAdornment, IconButton, Typography, Paper, Grid, List, ListItem, Divider, FormControl, InputLabel, Select, MenuItem } from "@mui/material";
import ClearIcon from '@mui/icons-material/Clear';
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import { useTranslation } from "react-i18next";
import { Controller, FieldErrors, FieldValues, useForm } from 'react-hook-form';
import { idealBanks, idealIcon } from '../../../../constants/banking';

interface IProps {
    control: any;
}
export default ({ control }: IProps) => {
    const { t } = useTranslation();


    return (
        <>
            <Grid item xs={12} style={{ marginTop: 6 }}>
                <Typography variant="h5">{t("Contact")}</Typography>
                <Divider />
            </Grid>
            <Grid item xs={12} sm={6}>
                <Controller
                    name="phoneNumber"
                    control={control}
                    rules={{ required: t("What is your garage telephone number?") }}
                    defaultValue={""}
                    render={({ field, fieldState: { error } }) => (
                        <TextField
                            {...field}
                            fullWidth
                            size="small"
                            label={t("Telephone number")}
                            variant="outlined"
                            error={!!error}
                            helperText={error ? t(error.message as string) : undefined}
                        />
                    )}
                />
            </Grid>
            <Grid item xs={12} sm={6}>
                <Controller
                    name="whatsappNumber"
                    control={control}
                    defaultValue={""}
                    render={({ field }) => (
                        <TextField
                            {...field}
                            fullWidth
                            size="small"
                            label={t("Whatsapp number")}
                            variant="outlined"
                        />
                    )}
                />
            </Grid>
            <Grid item xs={12} sm={6}>
                <Controller
                    name="emailAddress"
                    control={control}
                    rules={{ required: t("What is your garage email?") }}
                    defaultValue={""}
                    render={({ field, fieldState: { error } }) => (
                        <TextField
                            {...field}
                            fullWidth
                            size="small"
                            label={t("Email")}
                            variant="outlined"
                            error={!!error}
                            helperText={error ? t(error.message as string) : undefined}
                        />
                    )}
                />
            </Grid>
        </>
    );
}
