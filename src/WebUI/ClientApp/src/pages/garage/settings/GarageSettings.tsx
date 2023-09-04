import React, { Dispatch, memo, useEffect, useState } from "react";
import { Box, Breadcrumbs, Button, CircularProgress, Container, Dialog, DialogContent, DialogContentText, DialogTitle, Divider, Drawer, FormControl, Grid, Hidden, IconButton, InputAdornment, InputLabel, Link, List, ListItem, ListItemButton, ListItemIcon, ListItemText, MenuItem, Paper, Select, TextField, Toolbar, Tooltip, Typography, useMediaQuery, useTheme } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import { Controller, FieldValues, UseFormSetError, useForm } from "react-hook-form";
import { showOnError, showOnSuccess } from "../../../redux/slices/statusSnackbarSlice";
import { useDispatch } from "react-redux";
import GeneralSection from "./components/GeneralSection";
import BankingSection from "./components/BankingSection";
import ContactSection from "./components/ContactSection";
import { COLORS } from "../../../constants/colors";
import useGarage from "./useGarage";
// own imports

interface IProps {
}

export default ({ }: IProps) => {// TODO: want to use it in future? >> memo()
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const dispatch = useDispatch();
    const location = useLocation();
    const { t } = useTranslation();
    const { garage_guid } = useParams();
    const queryParams = new URLSearchParams(location.search);
    const notFound = queryParams.get('garage_notfound');

    const { reset, handleSubmit, control, formState: { errors }, setError, setValue } = useForm();
    const { loading, isError, createGarage, updateGarageSettings, garageSettings } = useGarage(reset, setError, notFound == "true", garage_guid);

    const onSubmit = async (data: any) => {
        if (notFound) {
            if (t("Select a bank...").match(data.bankName)) {
                setError("bankName", {
                    type: "manual",
                    message: t("Bank name is required.")
                });

                return;
            }
            else if (!data.longitude) {
                dispatch(showOnError(t("Select a address, it's required.")));
                setError("address", {
                    type: "manual",
                    message: t("Select a address, it's required.")
                });

                return;
            }

            createGarage(data);
        } else {
            updateGarageSettings(data);
        }
    }

    return (
        <>
            <Box py={4}>
                <Typography variant="h4" gutterBottom>
                    {t("GarageSettingsHeader.Title")}
                    <Tooltip title={t("GarageSettingsHeader.Description")}>
                        <IconButton size="small">
                            <InfoOutlinedIcon fontSize="inherit" />
                        </IconButton>
                    </Tooltip>
                </Typography>
            </Box>
            <Divider style={{ marginBottom: "20px" }} />
            <form onSubmit={handleSubmit(onSubmit)}>
                <Grid container spacing={2}>
                    <GeneralSection errors={errors} control={control} setFormValue={setValue} />
                    <ContactSection errors={errors} control={control} />
                    <BankingSection errors={errors} control={control} />
                    <Grid item xs={12}>
                        <Box display="flex" justifyContent="center" alignItems="center" height="90px">
                            {loading ?
                                <Button
                                    fullWidth={isMobile}
                                    variant="contained"
                                    disabled
                                    style={{ color: 'white', padding: '10px 30px' }}
                                >
                                    <CircularProgress size={24} color="inherit" />
                                </Button>
                                : isError ?
                                    <div>
                                        Error fetching garage settings
                                    </div>
                                    :
                                    <Button
                                        fullWidth={isMobile}
                                        type="submit"
                                        variant="contained"
                                        sx={{
                                            backgroundColor: COLORS.BLUE,
                                            color: 'white',
                                            padding: isMobile ? '10px 0' : '10px 30px',
                                            textTransform: 'uppercase',
                                            fontWeight: 'bold',
                                            '&:hover': { backgroundColor: COLORS.HOVERED_BLUE }
                                        }}
                                    >
                                        {notFound ? t("Register") : t("Save")}
                                    </Button>
                            }
                        </Box>
                    </Grid>
                </Grid>
            </form>
        </>
    );
};
