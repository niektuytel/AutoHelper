import React, { useState } from "react";
import { Box, Container, InputAdornment, TextField, IconButton, Button, Hidden, ListItem, List, useTheme, useMediaQuery, Autocomplete, CircularProgress } from "@mui/material";
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


// own imports
import { HashValues } from "../../../i18n/HashValues";
import { useDispatch } from "react-redux";
import { showOnError } from "../../../redux/slices/statusSnackbarSlice";
import { GarageLookupDto, GarageSearchClient, PaginatedListOfGarageLookupDto } from "../../../app/web-api-client";
import { useQueryClient } from "react-query";



interface IProps
{
    license_plate: string,
    latitude: number,
    longitude: number,
    in_km_range: number,
    page_size: number,
    onSearchExecuted: (data: PaginatedListOfGarageLookupDto) => void;
}

export default ({ license_plate, latitude, longitude, in_km_range, page_size, onSearchExecuted }: IProps) => {
    var hash = window.location.hash.length == 0 ? HashValues.default : window.location.hash;
    const navigate = useNavigate();
    const theme = useTheme();
    const dispatch = useDispatch();
    const isMobile = useMediaQuery(theme.breakpoints.down('md'));
    const splitted_hash = hash.split("_")[0];
    const { t } = useTranslation();

    const [isFocused, setIsFocused] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [value, setValue] = useState("");
    const [suggestions, setSuggestions] = React.useState<readonly GarageLookupDto[]>([]);
    const useGarageSearchClient = new GarageSearchClient(process.env.PUBLIC_URL);
    const queryClient = useQueryClient();

    const fetchGaragesData = async (autocomplete: string): Promise<PaginatedListOfGarageLookupDto> => {
        try {
            const response = await useGarageSearchClient.searchGarages(
                license_plate,
                latitude,
                longitude,
                in_km_range,
                1,
                page_size,
                autocomplete
            );
            return response;
        } catch (response: any) {
            throw response;
        }
    }

    const handleInput = async (e: any) => {
        setIsLoading(true);

        setValue(e.target.value);
        var data = await fetchGaragesData(e.target.value);
        setSuggestions(data?.items || []);

        // Overwrite the cached data with the new data
        if (data?.items && data?.items?.length > 0)
        {
            // Trigger the callback to notify the parent about the executed search
            onSearchExecuted(data);
        }

        setIsLoading(false);
    }

    const handleClearInput = () => {
        setValue("");
        setSuggestions([]);
    };

    const ref = useOnclickOutside(() => handleClearInput());

    return <>
        <Box position="relative">

            <TextField
                fullWidth
                autoComplete="new-password" // Use this line instead of autoComplete="off", because it is not working
                autoFocus={true}
                value={value}
                onFocus={() => setIsFocused(true)}
                onBlur={() => setIsFocused(false)}
                onChange={handleInput}
                variant="outlined"
                size="small"
                placeholder={t("Zoek op garage naam...")}
                InputProps={{
                    endAdornment: (
                        <InputAdornment position="end">
                            {isLoading ? (
                                <CircularProgress size={20} sx={{ mr: 2 }} />
                            ) : (
                                <>
                                    {value.length > 0 && (suggestions.length === 0 && isFocused) &&
                                        <Typography variant="body2" color="textSecondary" sx={{ mr: 1 }} >
                                                {t('No results')}
                                        </Typography>
                                    }
                                    {value.length > 0 ?
                                        <IconButton onClick={handleClearInput} style={{ marginRight: "10px" }}>
                                            <ClearIcon color="action" />
                                        </IconButton>
                                            :
                                        <IconButton style={{ marginRight: "10px" }}>
                                            <SearchIcon />
                                        </IconButton>
                                    }
                                </>
                            )}
                        </InputAdornment>
                    ),
                    style: {
                        fontSize: '1.2em',
                        paddingRight: '0',
                        backgroundColor: 'white'
                    }
                }}
            />
        </Box>
    </>
}



