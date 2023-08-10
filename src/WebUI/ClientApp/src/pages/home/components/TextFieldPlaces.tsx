import React from "react";
import { Box, Container, InputAdornment, TextField, IconButton, Button, Hidden } from "@mui/material";
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import usePlacesAutocomplete, {
    getGeocode,
    getLatLng,
    HookArgs,
} from "use-places-autocomplete";
import useOnclickOutside from "react-cool-onclickoutside";


// own imports
import { HashValues } from "../../../i18n/HashValues";



interface IProps {
}
// Google maps api key: AIzaSyDkY5sdYxA4lr7uqZ9vFxnwWPGf2NeIf08

export default ({  }: IProps) => {
    var hash = window.location.hash.length == 0 ? HashValues.default : window.location.hash;
    const navigate = useNavigate();
    const splitted_hash = hash.split("_")[0];
    const { t } = useTranslation();

    //const [suggestions, setSuggestions] = React.useState<string[]>([]);
    //const [inputValue, setInputValue] = React.useState<string>("");

    /////////////////////////

    // settings for google places autocomplete
    var request: HookArgs = {
        requestOptions: {
            componentRestrictions: { country: 'nl' },
            types: ['address']
        },
        debounce: 300,
    };
    const {
        ready,
        value,
        suggestions: { status, data },
        setValue,
        clearSuggestions,
    } = usePlacesAutocomplete(request);
    const ref = useOnclickOutside(() => handleClearInput());

    const handleInput = (e: any) => {
        // Update the keyword of the input element
        setValue(e.target.value);
    };

    const handleSelect = (suggestion: any) => {
        console.log(suggestion)
        // When user selects a place, we can replace the keyword without request data from API
        // by setting the second parameter to "false"
        setValue(suggestion.structured_formatting.main_text, false);
        clearSuggestions();

        // Get latitude and longitude via utility functions
        getGeocode({ address: suggestion.description }).then((results) => {
            const { lat, lng } = getLatLng(results[0]);
            console.log("📍 Coordinates: ", { lat, lng });
        });
    };

    ///////////////////////////
    const handleClearInput = () => {
        clearSuggestions();
    };

    const handleSearch = () => {
        // Handle the search functionality here
        // For now, let's just console.log the input value
        console.log(value);
    }


            //ref={ref}
    return <>
        <TextField
            fullWidth
            autoComplete="new-password" // Use this line instead of autoComplete="off", because it is not working
            autoFocus={true}
            value={value}
            onChange={handleInput}
            disabled={!ready}
            variant="outlined"
            placeholder="Adres, b.v. Amstelplein 10"
            InputProps={{
                startAdornment: (
                    <InputAdornment position="start">
                        <LocationOnOutlinedIcon color="action" />
                    </InputAdornment>
                ),
                endAdornment: (
                    <InputAdornment position="end">
                        <IconButton onClick={handleClearInput}>
                            <ClearIcon color="action" />
                        </IconButton>
                        <Hidden xlDown implementation="css">
                            <IconButton onClick={handleSearch}>
                                <SearchIcon />
                            </IconButton>
                        </Hidden>
                        <Hidden lgUp implementation="css">
                            {data.length >= 1 && (
                                <Button
                                    onClick={handleSearch}
                                    style={{
                                        backgroundColor: '#1B97F0',
                                        color: 'white',
                                        borderRadius: '4px',
                                        height: '100%',
                                        borderTopLeftRadius: '0',
                                        borderBottomLeftRadius: '0',
                                        margin: '0',
                                        padding: '0',
                                    }}
                                >
                                    Search
                                </Button>
                            )}
                        </Hidden>
                    </InputAdornment>
                ),
                style: {
                    height: '50px',
                    fontSize: '1.2em',
                    paddingRight: '0',
                }
            }}
        />
        {console.log(data)}
        {status == "OK" && ready &&
            <Box
                mt={1} boxShadow={2} width="100%" >
                {data.map((suggestion) => {
                    const { place_id, structured_formatting: { main_text, secondary_text } } = suggestion;
                    return (
                        <Box key={place_id} p={1} borderBottom="1px solid #ddd" onClick={() => handleSelect(suggestion)}>
                            {main_text}_{secondary_text}
                        </Box>
                    );
                })}
            </Box>
        }
    </>
}



