import React, { Dispatch, SetStateAction } from 'react';
import { TextField, Box, InputAdornment, IconButton, Typography, Paper, Grid, List, ListItem, Divider, FormControl, InputLabel, Select, MenuItem, Tooltip } from "@mui/material";
import ClearIcon from '@mui/icons-material/Clear';
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import { useTranslation } from "react-i18next";
import { Controller, FieldErrors, FieldValues, useForm } from 'react-hook-form';
import { idealBanks, idealIcon } from '../../../../constants/banking';
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';

interface BankingSectionProps {
    control: any;
}
export default ({ control }: BankingSectionProps) => {
    const { t } = useTranslation();


    return (
        <>
            <Grid item xs={12} style={{ marginTop: 3 }}>
                <Typography variant="h6" gutterBottom>
                    {t("GarageAccount.Conversation.Title")}
                    <Tooltip title={t("GarageAccount.Conversation.Description")}>
                        <IconButton size="small">
                            <InfoOutlinedIcon fontSize="inherit" />
                        </IconButton>
                    </Tooltip>
                </Typography>
                <Divider />
            </Grid>
            <Grid item xs={12} sm={6}>
                <Controller
                    name="conversationWhatsappNumber"
                    control={control}
                    defaultValue={""}
                    render={({ field, fieldState: { error } }) => (
                        <TextField
                            {...field}
                            fullWidth
                            size="small"
                            label={t("Whatsapp number")}
                            variant="outlined"
                            type="tel"
                            error={!!error}
                            helperText={error ? t(error.message as string) : undefined}
                        />
                    )}
                />
            </Grid>
            <Grid item xs={12} sm={6}>
                <Controller
                    name="conversationEmail"
                    control={control}
                    defaultValue={""}
                    render={({ field, fieldState: { error } }) => (
                        <TextField
                            {...field}
                            fullWidth
                            size="small"
                            label={t("Email")}
                            variant="outlined"
                            type="email"
                            error={!!error}
                            helperText={error ? t(error.message as string) : undefined}
                        />
                    )}
                />
            </Grid>
        </>
    );
}
