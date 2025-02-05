﻿import React, { Dispatch, SetStateAction } from 'react';
import { TextField, Box, InputAdornment, IconButton, Typography, Paper, Grid, List, ListItem, Divider, FormControl, InputLabel, Select, MenuItem } from "@mui/material";
import ClearIcon from '@mui/icons-material/Clear';
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import { useTranslation } from "react-i18next";
import { Controller, FieldErrors, FieldValues } from 'react-hook-form';

// custom imports
import { idealBanks, idealIcon } from '../../../../constants/banking';

//if (t("Select a bank...").match(data.bankName)) {
//    setError("bankName", {
//        type: "manual",
//        message: t("Select a bank...")
//    });

//    return;
//}
//else

//command.bankingDetails = new BriefBankingDetailsDto({
//    bankName: data.bankName,
//    kvKNumber: data.kvKNumber,
//    accountHolderName: data.accountHolderName,
//    iban: data.iban
//});

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
            {/* Banking Details Header */}
            <Grid item xs={12} style={{ marginTop: "30px" }}>
                <Typography variant="h5">{t("Banking Details")}</Typography>
                <Divider />
            </Grid>
            <Grid item xs={12} sm={6}>
                <Controller
                    name="kvKNumber"
                    control={control}
                    defaultValue={""}
                    rules={{ required: t("What is the KVK-number of your garage?") }}
                    render={({ field }) => (
                        <TextField
                            {...field}
                            fullWidth
                            size="medium"
                            label={t("KVK-number")}
                            variant="outlined"
                            error={Boolean(errors.kvKNumber)}
                            helperText={errors.kvKNumber ? t(errors.kvKNumber.message as string) : undefined}
                        />
                    )}
                />
            </Grid>
            <Grid item xs={12} sm={6}>
                <Controller
                    name="bankName"
                    control={control}
                    defaultValue={""}
                    rules={{ required: t("Select a bank...") }}
                    render={({ field }) => (
                        <FormControl
                            fullWidth
                            size="medium"
                            variant="outlined"
                            error={Boolean(errors.bankName)}
                        >
                            <Select
                                {...field}
                                label=""
                                displayEmpty
                                sx={{
                                    padding: 0,
                                    minHeight: "56px",
                                    '& .MuiSelect-select': {
                                        padding: 0,
                                        minHeight: "56px",
                                    },
                                }}
                                renderValue={(value) => {
                                    return (
                                        <div style={{
                                            display: 'flex',
                                            alignItems: 'center',
                                            flexWrap: 'wrap',
                                        }}>
                                            <img
                                                src={idealBanks[value as keyof typeof idealBanks] || idealIcon}
                                                alt={value}
                                                style={{ width: '32px', height: '32px', margin: "12px" }}
                                            />
                                            <span>
                                                {value || t("Select a bank...")}
                                            </span>
                                        </div>
                                    );
                                }}
                            >
                                <MenuItem value="">{t("Select a bank...")}</MenuItem>
                                {Object.keys(idealBanks).map(bank => (
                                    <MenuItem key={bank} value={bank}>
                                        <div style={{
                                            display: 'flex',
                                            alignItems: 'center',
                                            flexWrap: 'wrap',
                                        }}>
                                            <img
                                                src={idealBanks[bank as keyof typeof idealBanks]}
                                                alt={bank}
                                                style={{ width: '32px', height: '32px', margin: "12px" }}
                                            />
                                            <span>
                                                {bank}
                                            </span>
                                        </div>
                                    </MenuItem>
                                ))}
                            </Select>
                            {errors.bankName && <Typography variant="caption" color="error">{t(errors.bankName.message as string)}</Typography>}
                        </FormControl>
                    )}
                />
            </Grid>
            <Grid item xs={12} sm={6}>
                <Controller
                    name="accountHolderName"
                    control={control}
                    defaultValue={""}
                    rules={{ required: t("What is the name of the account holder?") }}
                    render={({ field }) => (
                        <TextField
                            {...field}
                            fullWidth
                            size="medium"
                            label={t("Account holder name")}
                            variant="outlined"
                            error={Boolean(errors.accountHolderName)}
                            helperText={errors.accountHolderName ? t(errors.accountHolderName.message as string) : undefined}
                        />
                    )}
                />
            </Grid>
            <Grid item xs={12} sm={6}>
                <Controller
                    name="iban"
                    control={control}
                    defaultValue={""}
                    rules={{ required: t("IBAN is required!") }}
                    render={({ field }) => (
                        <TextField
                            {...field}
                            fullWidth
                            size="medium"
                            label={t("IBAN")}
                            variant="outlined"
                            error={Boolean(errors.iban)}
                            helperText={errors.iban ? t(errors.iban.message as string) : undefined}
                        />
                    )}
                />
            </Grid>
        </>
    );
}

