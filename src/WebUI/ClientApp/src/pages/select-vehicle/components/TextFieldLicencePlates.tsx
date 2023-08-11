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
}

export default ({ }: IProps) => {
    var hash = window.location.hash.length == 0 ? HashValues.default : window.location.hash;
    const navigate = useNavigate();
    const splitted_hash = hash.split("_")[0];
    const { t } = useTranslation();

    // TODO: include '-' when needed on textfield
    
    const [value, setValue] = React.useState<string>("");
    const handleInput = (e: any) => setValue(e.target.value);
    const ref = useOnclickOutside(() => handleClearInput());

    const handleClearInput = () => {
        setValue("");
    };

    const handleSearch = () => {
        // TODO: get vehicle information on licence-plate (=== value)
        // If valid, 
        navigate(`/select-vehicle/${value}`)

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
            variant="outlined"
            placeholder="Kenteken, b.v. 87-GRN-6"
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
                        <Hidden mdUp>
                            <IconButton onClick={handleSearch} style={{ marginRight: "10px" }}>
                                    <SearchIcon />
                            </IconButton>
                        </Hidden>
                        <Hidden mdDown>
                            <Button
                                onClick={handleSearch}
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
                                    Zoek
                            </Button>
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
    </>
}



