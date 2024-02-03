import React from "react";
import { Box, InputAdornment, TextField, IconButton, Button, Hidden, ListItem, List, useTheme, useMediaQuery } from "@mui/material";
import { Paper, Typography, Grid } from '@mui/material';
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import SearchIcon from '@mui/icons-material/Search';
import ArrowBackIosIcon from '@mui/icons-material/ArrowBackIos';
import ClearIcon from '@mui/icons-material/Clear';
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate } from "react-router-dom";
import usePlacesAutocomplete, { getGeocode, getLatLng, Suggestion } from "use-places-autocomplete";
import useOnclickOutside from "react-cool-onclickoutside";
import { useDispatch } from "react-redux";

// own imports
import { showOnError } from "../../../redux/slices/statusSnackbarSlice";
import { ROUTES } from "../../../constants/routes";

interface IProps {
    licence_plate: string
}

export default ({ licence_plate }: IProps) => {
    const navigate = useNavigate();
    const location = useLocation();
    const theme = useTheme();
    const dispatch = useDispatch();
    const isMobile = useMediaQuery(theme.breakpoints.down('md'));
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
                country: "nl",// should stay on the netherlands
            }
        },
        debounce: 250
    });


    const handleSearch = (place_id: string) => {
        // TODO: change place id to something that the user can type in, (that we are only using the input:string)
        getGeocode({ placeId: place_id })
            .then(results => {

                // TODO: Set input string value to the the cookie: named 'recently_used_place'
                const { lat, lng } = getLatLng(results[0]);
                navigate(`${ROUTES.GARAGES}/${licence_plate}/${lat}/${lng}`, { state: { from: location } });
            })
            .catch(error => {
                dispatch(showOnError(t("Error on getting address location")));
                console.log("Error fetching geocode:", error);
            });
    }

    const handleInput = (e: any) => setValue(e.target.value);
    const handleSelect = (place_id: any, input: string) => {
        // TODO: change place id to something that the user can type in, (that we are only using the input:string)
        setValue(input, false);
        clearSuggestions();

        handleSearch(place_id)
    };

    const handleClearInput = () => {
        setValue("", false);
        clearSuggestions();
    };

    return <>
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
                    startAdornment: (
                        <InputAdornment position="start">
                            <LocationOnOutlinedIcon color="action" />
                        </InputAdornment>
                    ),
                    endAdornment: (
                        <InputAdornment position="end">
                            {value.length > 0 &&
                                <IconButton onClick={handleClearInput}>
                                    <ClearIcon color="action" />
                                </IconButton>
                            }
                            {data.length == 0 && <>
                                <Hidden mdUp>
                                    <IconButton style={{ marginRight: "10px" }}>
                                        <SearchIcon />
                                    </IconButton>
                                </Hidden>
                                <Hidden mdDown>
                                    <Button
                                        sx={{
                                            backgroundColor: '#1B97F0',
                                            color: 'white',
                                            borderRadius: '4px',
                                            height: '50px',
                                            borderTopLeftRadius: '0',
                                            borderBottomLeftRadius: '0',
                                            margin: '0',
                                            padding: '0',
                                            '&:hover': {
                                                backgroundColor: '#1888d9'
                                            }
                                        }}
                                    >
                                        <Typography variant="body1" textAlign="center">
                                            {t("search_camelcase")}
                                        </Typography>
                                    </Button>
                                </Hidden>
                            </>}
                        </InputAdornment>
                    ),
                    style: {
                        marginTop: isMobile ? '15px' : '15px',
                        height: '50px',
                        fontSize: '1.2em',
                        paddingRight: '0',
                        backgroundColor: 'white'
                    }
                }}
            />
            {status == "OK" && ready &&
                <Box position="absolute" width="100%" zIndex={2} mt="5px">
                    <Paper elevation={3}>
                        <List dense disablePadding>
                            {data.filter((x: Suggestion) => x.terms.length > 2).map((suggestion: Suggestion) => {
                                const { place_id, terms, types } = suggestion;

                                console.log(terms, types)
                                const input = terms.slice(0, -1).map((term:any) => term.value).join(', ');
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
    </>
}



