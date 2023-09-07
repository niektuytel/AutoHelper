import React, { Dispatch, memo, useEffect, useState } from "react";
import { Box, Breadcrumbs, Button, ButtonGroup, CircularProgress, Container, Dialog, DialogContent, DialogContentText, DialogTitle, Divider, Drawer, FormControl, Grid, Hidden, IconButton, InputAdornment, InputLabel, Link, List, ListItem, ListItemButton, ListItemIcon, ListItemText, MenuItem, Paper, Select, TextField, Toolbar, Tooltip, Typography, useMediaQuery, useTheme } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import { Controller, FieldValues, UseFormSetError, useForm } from "react-hook-form";
import { useDispatch } from "react-redux";

// own imports
import { showOnError, showOnSuccess } from "../../../redux/slices/statusSnackbarSlice";
import ProfileGeneralSection from "./components/ProfileGeneralSection";
import ProfileBankingSection from "./components/ProfileBankingSection";
import ProfileContactSection from "./components/ProfileContactSection";
import { COLORS } from "../../../constants/colors";
import useGarageSettings from "./useGarageSettings";
import ServicesGeneralSection from "./components/ServicesGeneralSection";
import ServicesDeliverySection from "./components/ServicesDeliverySection";
import { ROUTES } from "../../../constants/routes";
import { ROLES } from "../../../constants/roles";

// TODO: set all translations for this page

interface IProps {
}

export default ({ }: IProps) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const [activeSection, setActiveSection] = useState('profile');
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const location = useLocation();
    const { t } = useTranslation();
    
    // redirect when user is on confirmation step 1
    const userRole = localStorage.getItem('userRole');
    const confirmationStepIndex = Number(localStorage.getItem('confirmationStepIndex'));
    const notFound = (userRole == ROLES.GARAGE && confirmationStepIndex == 1)

    const { reset, handleSubmit, control, formState: { errors }, setError, setValue } = useForm();
    const { loading, isError, createGarage, updateGarageSettings, garageSettings } = useGarageSettings(reset, setError, notFound);


    // Update hash when location changes
    useEffect(() => {
        if (location.hash === "#services") {
            setActiveSection("services");
        } else {
            setActiveSection("profile");
        }
    }, [location.hash]);

    const handleSectionChange = (section: string) => {
        navigate(`#${section}`);
    }

    const onSubmit = async (data: any) => {
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

        if (notFound) {
            createGarage(data);
        } else {
            updateGarageSettings(data);
        }
    }

    return (
        <>
            <Box pt={4}>
                <Typography variant="h4" gutterBottom>
                    {t("GarageSettingsHeader.Title")}
                    <Tooltip title={t("GarageSettingsHeader.Description")}>
                        <IconButton size="small">
                            <InfoOutlinedIcon fontSize="inherit" />
                        </IconButton>
                    </Tooltip>
                </Typography>
            </Box>
            {!notFound ?
                <ButtonGroup sx={{paddingBottom: 1}}>
                    <Button variant={activeSection === "profile" ? "contained" : "outlined"} onClick={() => handleSectionChange('profile')}>
                        {t('Profile')}
                    </Button>
                    <Button variant={activeSection === "services" ? "contained" : "outlined"} onClick={() => handleSectionChange('services')}>
                        {t('Services')}
                    </Button>
                </ButtonGroup>
                :
                <Box py={3}></Box>
            }
            <Divider style={{ marginBottom: "20px" }} />
            <form onSubmit={handleSubmit(onSubmit)}>
                <Grid container spacing={2}>
                    {
                        activeSection === 'profile' ?
                            <>
                                <ProfileGeneralSection errors={errors} control={control} setFormValue={setValue} defaultLocation={garageSettings} />
                                <ProfileContactSection errors={errors} control={control} />
                                <ProfileBankingSection errors={errors} control={control} />
                            </>
                            :
                            <>
                                <ServicesGeneralSection errors={errors} control={control} />
                                <ServicesDeliverySection errors={errors} control={control} />
                            </>
                    }
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
