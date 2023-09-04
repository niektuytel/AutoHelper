import React, { Dispatch, SetStateAction } from 'react';
import { TextField, Box, InputAdornment, IconButton, Typography, Paper, Grid, List, ListItem, Divider, FormControl, InputLabel, Select, MenuItem } from "@mui/material";
import ClearIcon from '@mui/icons-material/Clear';
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import { useTranslation } from "react-i18next";
import { Controller, FieldErrors, FieldValues } from 'react-hook-form';
import { GarageSettings, GarageLocationItem } from '../../../../app/web-api-client';
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
            {/* Contact Information Header */}
            <Grid item xs={12} style={{ marginTop: "40px" }}>
                <Typography variant="h5">{t("Orders")}</Typography>
                <Divider />
            </Grid>
            <Grid item xs={12} sm={6}>
                {/*TODO: <Controller*/}
                {/*    name="phoneNumber"  // Refactored name*/}
                {/*    control={control}*/}
                {/*    rules={{ required: t("Phone Number is required!") }}*/}
                {/*    defaultValue={""}*/}
                {/*    render={({ field }) => (*/}
                {/*        <TextField*/}
                {/*            {...field}*/}
                {/*            fullWidth*/}
                {/*            label={t("Phone number")}*/}
                {/*            variant="outlined"*/}
                {/*            error={Boolean(errors.phoneNumber)}  // Refactored error check*/}
                {/*            helperText={errors.phoneNumber ? t(errors.phoneNumber.message as string) : undefined}*/}
                {/*        />*/}
                {/*    )}*/}
                {/*/>*/}
            </Grid>
        </>
    );
}
