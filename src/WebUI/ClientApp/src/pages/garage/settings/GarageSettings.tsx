import React, { useEffect, useState } from "react";
import { Box, Breadcrumbs, Button, CircularProgress, Container, Dialog, DialogContent, DialogContentText, DialogTitle, Divider, Drawer, FormControl, Grid, Hidden, IconButton, InputAdornment, InputLabel, Link, List, ListItem, ListItemButton, ListItemIcon, ListItemText, MenuItem, Paper, Select, TextField, Toolbar, Tooltip, Typography, useMediaQuery, useTheme } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import { BankingInfoItem, BusinessOwnerItem, ContactItem, CreateGarageItemCommand, GarageClient, GarageSettings, IGarageSettings, LocationItem, UpdateGarageItemSettingsCommand } from "../../../app/web-api-client";
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import { RoutesGarageSettings } from "../../../constants/routes";
import { Controller, useForm } from "react-hook-form";
import { setError, setSuccess } from "../../../redux/slices/statusSnackbarSlice";
import { useDispatch } from "react-redux";
import GeneralSection from "./components/GeneralSection";
import { idealBanks, idealIcon } from "../../../constants/banking";
import { blue } from '@mui/material/colors';
import BankingSection from "./components/BankingSection";
import ContactSection from "./components/ContactSection";
import { COLORS } from "../../../constants/colors";
// own imports



function initialGarageLocation(): LocationItem {
    const location = new LocationItem();
    location.country = "Netherlands";
    return location;
}

interface IProps {
}

export default ({ }: IProps) => {
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const dispatch = useDispatch();
    const garageClient = new GarageClient(process.env.PUBLIC_URL);
    const { handleSubmit, control, formState: { errors } } = useForm();
    const { garage_guid } = useParams();
    const location = useLocation();
    const navigate = useNavigate();
    const queryParams = new URLSearchParams(location.search);
    const notFound = queryParams.get('garage_notfound');
    const initialGarageSettings = new GarageSettings({
        name: "",
        email: "",
        phoneNumber: "",
        whatsAppNumber: "",
        location: initialGarageLocation(),
        bankingDetails: new BankingInfoItem(),
        contacts: []
    });
    
    const initialState = {
        isLoading: false,
        garageSettings: initialGarageSettings
    };
    const [state, setState] = useState(initialState);
    const { t } = useTranslation();

    useEffect(() => {
        if (!notFound) fetchGarageSettings();
    }, []);

    const fetchGarageSettings = async () => {
        setState(prev => ({ ...prev, isLoading: true }));

        garageClient.settings(garage_guid!)
            .then(response => {
                console.log("Garage settings:", response);
                setState(prev => ({ ...prev, garageSettings: response }));
            })
            .catch(error => {
                console.log(error.body)
                if (error.status === 404) {
                    dispatch(setError("Garage is not been found"));

                    console.log('Garage not found:', error);
                    navigate(`${RoutesGarageSettings(garage_guid!)}?garage_notfound=true`);
                }
                else {
                    // TODO: trigger snackbar
                    console.error("Error occurred:", error);
                }
            })
            .finally(() => {
                setState(prev => ({ ...prev, isLoading: false }));
            });
    };

    const createGarage = async (garageData: GarageSettings) => {
        setState(prev => ({ ...prev, isLoading: true }));

        var command = new CreateGarageItemCommand();
        command.name = garageData.name;
        command.location = garageData.location;
        command.phoneNumber = garageData.phoneNumber;
        command.whatsAppNumber = garageData.whatsAppNumber;
        command.email = garageData.email;
        command.bankingDetails = garageData.bankingDetails;

        console.log(command);
        garageClient.create(command)
            .then(response => {
                console.error(response);
                // TODO: handle the success response. Maybe show a success message or snackbar.
            })
            .catch(error => {
                console.error(error.message);
                dispatch(setSuccess("This is a success message!"));
                // TODO: trigger snackbar or show an error message to the user.
            })
            .finally(() => {
                setState(prev => ({ ...prev, isLoading: false }));
            });
    };

    const onSubmit = async (data: any) => {
        console.log(data);

        if (notFound) {
            createGarage(state.garageSettings);
        } else {
            dispatch(setError("MIssing functionality to update"));
            //updateGarageSettings(state.garageSettings);
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
                    <GeneralSection state={state} setState={setState} errors={errors} control={control} />
                    <ContactSection state={state} control={control} errors={errors} />
                    <BankingSection state={state} control={control} errors={errors} />
                    <Grid item xs={12}>
                        <Box display="flex" justifyContent="center" alignItems="center" height="90px">
                            {state.isLoading ? (
                                <Button
                                    fullWidth={isMobile}
                                    variant="contained"
                                    disabled
                                    style={{ color: 'white', padding: '10px 30px' }}
                                >
                                    <CircularProgress size={24} color="inherit" />
                                </Button>
                            ) : (
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
                            )}
                        </Box>
                    </Grid>
                </Grid>
            </form>
        </>
    );
}
