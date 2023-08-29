import React, { useState } from 'react';
import { LocationItem } from '../../../../app/web-api-client';
import { Box, Container, InputAdornment, TextField, IconButton, Button, Hidden, ListItem, List, FormControl } from "@mui/material";
import { Paper, Typography, Grid, ButtonBase } from '@mui/material';
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import SearchIcon from '@mui/icons-material/Search';
import ChevronRightOutlinedIcon from '@mui/icons-material/ChevronRightOutlined';
import ArrowBackIosIcon from '@mui/icons-material/ArrowBackIos';
import ClearIcon from '@mui/icons-material/Clear';
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import usePlacesAutocomplete, {
    getGeocode,
    getLatLng,
    HookArgs,
    Suggestion,
} from "use-places-autocomplete";
import useOnclickOutside from "react-cool-onclickoutside";

interface LocationProps {
    location?: LocationItem;
    onChange?: (location: LocationItem) => void;
}

const LocationSection: React.FC<LocationProps> = ({ location, onChange }) => {
    const [currentLocation, setCurrentLocation] = useState<LocationItem>(location || new LocationItem());

    //var hash = window.location.hash.length == 0 ? HashValues.default : window.location.hash;
    const navigate = useNavigate();
    //const splitted_hash = hash.split("_")[0];
    const { t } = useTranslation();

    const {
        ready,
        value,
        suggestions: { status, data },
        setValue,
        clearSuggestions,
    } = usePlacesAutocomplete({
        requestOptions: {
            types: ["address"],
            componentRestrictions: {
                // TODO: set country based on the 'useTanslation()'
                country: "nl",
            }
        },
        debounce: 250
    });


    const handleSearch = (place_id: string) => {
        // TODO: change place id to something that the user can type in, (that we are only using the input:string)
        getGeocode({ placeId: place_id })
            .then(results => {
                const { lat, lng } = getLatLng(results[0]);

                console.log(value);

                const updatedLocation: LocationItem = currentLocation;
                updatedLocation["latitude"] = lat;
                updatedLocation["longitude"] = lng;
                updatedLocation["address"] = value;

                // set location
                setCurrentLocation(updatedLocation);
                onChange && onChange(updatedLocation);
            })
            .catch(error => {
                // TODO: trigger an snackbar on redux, dispatch state with this error message (get it from the useTranslation)
                console.log("Error fetching geocode:", error);
            });
    }

    const handleInput = (e: any) => setValue(e.target.value);
    const handleSelect = (place_id: any, input: string) => {
        // TODO: change place id to something that the user can type in, (that we are only using the input:string)

        // When user selects a place, we can replace the keyword without request data from API
        // by setting the second parameter to "false"
        setValue(input, false);
        clearSuggestions();

        handleSearch(place_id)
    };

    const handleClearInput = () => {
        // When user selects a place, we can replace the keyword without request data from API
        // by setting the second parameter to "false"
        setValue("", false);
        clearSuggestions();
    };

    const ref = useOnclickOutside(() => handleClearInput());

    const handleBackNavigation = () => {
        navigate(`/select-vehicle`);
    };

    type LocationItemProperties = Pick<LocationItem, "longitude" | "latitude" | "address" | "city" | "postalCode" | "country">;

    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = event.target;
        const key = name as keyof LocationItemProperties;
        const updatedLocation: LocationItem = currentLocation;

        // Type guard for longitude and latitude
        if (key === "longitude" || key === "latitude") {
            updatedLocation[key] = parseFloat(value);
        } else {
            updatedLocation[key] = value;
        }

        // set location
        setCurrentLocation(updatedLocation);
        onChange && onChange(updatedLocation);
    };

    return (
        <>
            <Typography variant="h5" gutterBottom>
                Location Details
            </Typography>
            <Box marginBottom={2}>
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
                        ),
                        style: {
                            marginTop: '10px',
                            height: '50px',
                            fontSize: '1.2em',
                            paddingRight: '0',
                            backgroundColor: 'white'
                        }
                    }}
                />
                {status == "OK" && ready &&
                    <Box mt="5px" width="100%">
                        <Paper elevation={3}>
                            <Typography paddingLeft="20px" paddingTop="5px" variant="subtitle1" textAlign="left">
                                <b>{t("suggestions_camelcase")}</b>
                            </Typography>
                            <List dense disablePadding>
                                {data.filter((x: Suggestion) => x.terms.length > 2).map((suggestion: Suggestion) => {
                                    const { place_id, terms, types } = suggestion;

                                    console.log(terms, types)
                                    const input = terms.slice(0, -1).map((term: any) => term.value).join(', ');
                                    return (
                                        <ListItem
                                            button
                                            key={place_id}
                                            onClick={() => handleSelect(suggestion.place_id, input)}
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
                                                        {input}
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
            <Box marginBottom={2}>
                <TextField
                    fullWidth
                    label="PostalCode"
                    variant="outlined"
                    name="postalCode"
                    value={currentLocation.postalCode}
                    onChange={handleInputChange}
                    type="string"
                />
            </Box>
            <Box marginBottom={2}>
                <TextField
                    fullWidth
                    label="City"
                    variant="outlined"
                    name="city"
                    value={currentLocation.city}
                    onChange={handleInputChange}
                    type="string"
                />
            </Box>
        </>
    );
}

export default LocationSection;
