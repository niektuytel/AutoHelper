import React, { useEffect, useState } from "react";
import { Box, Breadcrumbs, Button, Container, Dialog, DialogContent, DialogContentText, DialogTitle, Divider, Drawer, FormControl, Grid, Hidden, IconButton, InputAdornment, Link, List, ListItem, ListItemButton, ListItemIcon, ListItemText, Paper, TextField, Toolbar, Tooltip, Typography } from "@mui/material";
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate, useParams } from "react-router-dom";
import { BankingInfoItem, BusinessOwnerItem, ContactItem, CreateGarageItemCommand, GarageClient, GarageSettings, IGarageSettings, LocationItem, UpdateGarageItemSettingsCommand } from "../../../app/web-api-client";
//import LocationSection from "./components/LocationSection";
//import BusinessOwnerSection from "./components/BusinessOwnerSection";
//import BankingInfoSection from "./components/BankingInfoSection";
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import SearchIcon from '@mui/icons-material/Search';
import ChevronRightOutlinedIcon from '@mui/icons-material/ChevronRightOutlined';
import ArrowBackIosIcon from '@mui/icons-material/ArrowBackIos';
import ClearIcon from '@mui/icons-material/Clear';
import { RoutesGarageSettings } from "../../../constants/routes";
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import usePlacesAutocomplete, {
    getGeocode,
    getLatLng,
    HookArgs,
    Suggestion,
} from "use-places-autocomplete";
// own imports

function initialGarageLocation(): LocationItem {
    const location = new LocationItem();
    location.country = "Netherlands";
    return location;
}

function updateLocationItem(location: LocationItem, updates: Partial<LocationItem>): Partial<LocationItem> {
    return {
        ...location,
        ...updates
    };
}

interface IProps {
}

export default ({ }: IProps) => {
    const garageClient = new GarageClient(process.env.PUBLIC_URL);
    const { garage_guid } = useParams();
    const location = useLocation();
    const navigate = useNavigate();
    const queryParams = new URLSearchParams(location.search);
    const notFound = queryParams.get('garage_notfound');
    const initialGarageSettings = new GarageSettings({
        name: "",
        location: initialGarageLocation(),
        businessOwner: new BusinessOwnerItem(),
        bankingDetails: new BankingInfoItem(),
        contacts: []
    });
    
    const initialState = {
        isLoading: false,
        garageSettings: initialGarageSettings
    };
    const [state, setState] = useState(initialState);
    const { t } = useTranslation();

    const {
        ready,
        value,
        suggestions: { status, data },
        setValue,
        clearSuggestions,
    } = usePlacesAutocomplete({
        requestOptions: {
            types: ["geocode"],
            componentRestrictions: {
                // TODO: set country based on the 'useTanslation()'
                country: "nl",
            }
        },
        debounce: 250
    });


    const handleSearch = (place_id: string, address: string, city: string, postalCode: string) => {
        getGeocode({ placeId: place_id })
            .then(results => {
                const { lat, lng } = getLatLng(results[0]);
                handleSettingsLocationChange(({
                    latitude: lat,
                    longitude: lng,
                    address: address,
                    city: city,
                    postalCode: postalCode.length == 0 ? state.garageSettings.location?.postalCode : postalCode
                }));
            })
            .catch(error => {
                // TODO: trigger an snackbar on redux, dispatch state with this error message (get it from the useTranslation)
                console.log("Error fetching geocode:", error);
            });
    }

    const handleInput = (e: any) => setValue(e.target.value);
    const handleSelect = (place_id: any, address: string, city: string, postalCode: string) => {
        // When user selects a place, we can replace the keyword without request data from API
        // by setting the second parameter to "false"
        setValue(`${address}, ${city}`, false);
        clearSuggestions();

        handleSearch(place_id, address, city, postalCode)
    };

    const handleClearInput = () => {
        // When user selects a place, we can replace the keyword without request data from API
        // by setting the second parameter to "false"
        setValue("", false);
        clearSuggestions();
    };

    //const ref = useOnclickOutside(() => handleClearInput());

    //const handleBackNavigation = () => {
    //    navigate(`/select-vehicle`);
    //};

    useEffect(() => {
        if (!notFound) fetchGarageSettings(garage_guid!);
    }, []);

    const fetchGarageSettings = async (garageId: string) => {
        setState(prev => ({ ...prev, isLoading: true }));

        garageClient.settings(garage_guid!)
            .then(response => {
                if (response) {
                    console.log("Response received:", response);
                    setState(prev => ({ ...prev, garageSettings: response }));
                } else {
                    // TODO: trigger snackbar
                    console.error("Failed to get vehicle by license plate");
                }
            })
            .catch(error => {
                if (error.status === 404) {
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
        command.id = garage_guid;
        command.name = garageData.name;
        command.location = garageData.location;
        command.businessOwner = garageData.businessOwner;
        command.bankingDetails = garageData.bankingDetails;

        console.log(command);
        garageClient.create(command)
            .then(response => {
                console.error(response);
                // TODO: handle the success response. Maybe show a success message or snackbar.
            })
            .catch(error => {
                console.error(error.message);
                // TODO: trigger snackbar or show an error message to the user.
            })
            .finally(() => {
                setState(prev => ({ ...prev, isLoading: false }));
            });
    };

    const updateGarageSettings = async (garageData: GarageSettings) => {
        setState(prev => ({ ...prev, isLoading: true }));

        var command = new UpdateGarageItemSettingsCommand();
        command.id = garage_guid;
        command.name = garageData.name;
        command.location = garageData.location;
        command.businessOwner = garageData.businessOwner;
        command.bankingDetails = garageData.bankingDetails;

        garageClient.updateSettings(command)
            .then(response => {
                // TODO: handle the success response. Maybe show a success message or snackbar.
            })
            .catch(error => {
                console.error("Error occurred during save:", error);
                // TODO: trigger snackbar or show an error message to the user.
            })
            .finally(() => {
                setState(prev => ({ ...prev, isLoading: false }));
            });
    };

    const handleSettingsChange = (updatedValues: Partial<GarageSettings>) => {
        const updatedGarageSettings = new GarageSettings({
            ...state.garageSettings,
            ...updatedValues
        });

        setState(prev => ({ ...prev, garageSettings: updatedGarageSettings }));
    };

    const handleSettingsLocationChange = (updatedValues: Partial<LocationItem>) => {
        const updatedLocation = updateLocationItem(state.garageSettings.location!, updatedValues);
        const updatedGarageSettings = new GarageSettings({
            ...state.garageSettings,
            location: updatedLocation as LocationItem  // typecasting since `updateLocationItem` returns Partial<LocationItem>
        });

        console.log(updatedGarageSettings);

        setState(prev => ({ ...prev, garageSettings: updatedGarageSettings }));
    };

    const handleSave = async () => {
        if (notFound) {
            createGarage(state.garageSettings);
        } else {
            updateGarageSettings(state.garageSettings);
        }
    }

    return (
        <Container maxWidth="lg" sx={{ minHeight:"100vh" }}>
            {/* Header */}
            <Box py={4}>
                <Typography variant="h4" gutterBottom>
                    Garage Instellingen
                    <Tooltip title="Information regarding the garage's basic details">
                        <IconButton size="small">
                            <InfoOutlinedIcon fontSize="inherit" />
                        </IconButton>
                    </Tooltip>
                </Typography>
            </Box>
            <Divider style={{ marginBottom: "20px" }} />
            <Grid container spacing={2}>
                <Grid item xs={12}>
                    <TextField
                        fullWidth
                        label="Name"
                        variant="outlined"
                        value={state.garageSettings.name}
                        onChange={e => handleSettingsChange({ name: e.target.value })}
                    />
                </Grid>
                <Grid item xs={12} sm={6}>
                    <Box position="relative">
                        <TextField
                            fullWidth
                            autoComplete="new-password" // Use this line instead of autoComplete="off", because it is not working
                            autoFocus={true}
                            value={value}
                            onChange={handleInput}
                            disabled={!ready}
                            variant="outlined"
                            placeholder={t("search_places_placeholder")}
                            InputProps={{
                                endAdornment: (
                                    <InputAdornment position="end">
                                        {value.length > 0 &&
                                            <IconButton onClick={handleClearInput}>
                                                <ClearIcon color="action" />
                                            </IconButton>
                                        }
                                    </InputAdornment>
                                )
                            }}
                        />
                        {status == "OK" && ready &&
                            <Box position="absolute" top="60px" width="100%" zIndex={2} mt="5px">
                                <Paper elevation={3}>
                                    <Typography paddingLeft="20px" paddingTop="5px" variant="subtitle1" textAlign="left">
                                        <b>{t("suggestions_camelcase")}</b>
                                    </Typography>
                                    <List dense disablePadding>
                                        {data.filter((x: Suggestion) => (x.terms.length > 2 && x.types.includes("premise") && x.types.includes("geocode"))).map((suggestion: Suggestion) => {
                                            const { place_id, terms } = suggestion;
                                            console.log(suggestion);

                                            const address = `${terms[0].value} ${terms[1].value}`;
                                            const city = terms[terms.length - 2].value;
                                            const postalCode = terms.length == 5 ? terms[terms.length - 3].value : "";
                                            return (
                                                <ListItem
                                                    button
                                                    key={place_id}
                                                    onClick={() => handleSelect(suggestion.place_id, address, city, postalCode)}
                                                    style={{ padding: '8px 13px', justifyContent: 'start' }}
                                                >
                                                    <Grid container spacing={1} alignItems="center">
                                                        <Grid item>
                                                            <InputAdornment position="start">
                                                                <LocationOnOutlinedIcon color="action" />
                                                            </InputAdornment>
                                                        </Grid>
                                                        <Grid item xs zeroMinWidth>
                                                            <Typography noWrap>
                                                                {`${terms[0].value} ${terms[1].value}, ${terms[2].value}`}
                                                            </Typography>
                                                        </Grid>
                                                    </Grid>
                                                </ListItem>
                                            );
                                        })}
                                    </List>
                                </Paper>
                            </Box>
                        }
                    </Box>
                </Grid>
                <Grid item xs={12} sm={6}>
                    <TextField
                        fullWidth
                        label="PostalCode"
                        variant="outlined"
                        value={state.garageSettings.location!.postalCode}
                        onChange={e => handleSettingsLocationChange({ postalCode: e.target.value })}
                    />
                </Grid>
                <Grid item xs={12}>
                    {state.isLoading ?
                        <Button
                            variant="contained"
                            color="primary"
                            disabled>
                            Loading...
                        </Button>
                        :
                        <Button
                            variant="contained"
                            color="primary"
                            onClick={handleSave}>
                            Save
                        </Button>
                    }
                </Grid>
            </Grid>
        </Container>
    );
}
