import React from "react";
import { Box, Container, InputAdornment, TextField, IconButton, Button, Hidden, ListItem, List } from "@mui/material";
import { Paper, Typography, Grid, ButtonBase } from '@mui/material';
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import SearchIcon from '@mui/icons-material/Search';
import ChevronRightOutlinedIcon from '@mui/icons-material/ChevronRightOutlined';
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


// own imports
import { HashValues } from "../../../i18n/HashValues";



interface IProps {
    licence_plate: string
}

export default ({ licence_plate }: IProps) => {
    var hash = window.location.hash.length == 0 ? HashValues.default : window.location.hash;
    const navigate = useNavigate();
    const splitted_hash = hash.split("_")[0];
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

                // TODO: Set input string value to the the cookie: named 'recently_used_place'

                const { lat, lng } = getLatLng(results[0]);
                navigate(`/select-garage/${licence_plate}/${lat}/${lng}`);
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


    // TODO: Set autofill the value of the textfield from the cookie
    // TODO: search or trigger: handleSearch when press enter button.
    return <>
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
                        {data.length == 0 ? <>
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
                        </>
                        : 
                        <>
                            <Hidden mdUp>
                                <IconButton onClick={() => handleSearch(value)} style={{ marginRight: "10px" }}>
                                    <ChevronRightOutlinedIcon />
                                </IconButton>
                            </Hidden>
                            <Hidden mdDown>
                                <Button
                                    onClick={() => handleSearch(value)}
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
                                            {t("next_camelcase")}
                                        </Typography>
                                </Button>
                            </Hidden>
                        </>}
                    </InputAdornment>
                ),
                style: {
                    marginTop: '60px',
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
    </>
}



