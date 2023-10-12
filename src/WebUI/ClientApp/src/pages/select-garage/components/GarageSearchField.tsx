import React, { useEffect, useState } from "react";
import { Box, Container, InputAdornment, TextField, IconButton, Button, Hidden, ListItem, List, useTheme, useMediaQuery, Autocomplete, CircularProgress, Chip, Skeleton } from "@mui/material";
import { Paper, Typography, Grid, ButtonBase } from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";

// own imports
import { HashValues } from "../../../i18n/HashValues";
import { GarageLookupDto, GarageSearchClient, GarageServiceType, PaginatedListOfGarageLookupDto } from "../../../app/web-api-client";
import useGarageSearchServiceTypes from "../useGarageSearchServiceTypes";



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
    const queryParams = new URLSearchParams(window.location.search);
    const { t } = useTranslation();

    const [isFocused, setIsFocused] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [value, setValue] = useState("");
    const [filters, setFilters] = useState<string[]>([]);
    const [suggestions, setSuggestions] = React.useState<readonly GarageLookupDto[]>([]);

    const { loading, garageServiceTypes } = useGarageSearchServiceTypes(license_plate!);
    const useGarageSearchClient = new GarageSearchClient(process.env.PUBLIC_URL);

    useEffect(() => {
        if (queryParams.has("filters")) {
            const filters = queryParams.get("filters")?.split(",");
            setFilters(filters || []);
        }
        
    }, [window.location.search]);

    const fetchGaragesData = async (autocomplete: string, filterValues: string[]): Promise<PaginatedListOfGarageLookupDto> => {
        try {
            const response = await useGarageSearchClient.searchGarages(
                license_plate,
                latitude,
                longitude,
                in_km_range,
                1,
                page_size,
                autocomplete,
                filterValues
            );
            return response;
        } catch (response: any) {
            throw response;
        }
    }

    const handleInput = async (e: any) => {
        setIsLoading(true);

        setValue(e.target.value);
        var data = await fetchGaragesData(e.target.value, filters);
        setSuggestions(data?.items || []);

        // Overwrite the cached data with the new data
        if (data?.items && data?.items?.length > 0)
        {
            // Trigger the callback to notify the parent about the executed search
            onSearchExecuted(data);
        }

        setIsLoading(false);
    }

    const handleChipClick = async (filterKey: string) => {
        setIsLoading(true);

        var filterValues = filters;
        if (filters.includes(filterKey)) {
            filterValues = filterValues.filter(f => f !== filterKey);
        } else {
            filterValues = [...filterValues, filterKey];
        }

        // Refetch the data
        const data = await fetchGaragesData(value, filterValues);
        setSuggestions(data?.items || []);
        setFilters(filterValues);

        // Overwrite the cached data with the new data
        if (data?.items && data?.items?.length > 0) {
            // Trigger the callback to notify the parent about the executed search
            onSearchExecuted(data);
        }

        // Update the URI with the filters
        const filtersRecord: Record<string, string> = {};
        filtersRecord["filters"] = filterValues.join(",");
        const newQueryParams = new URLSearchParams(filtersRecord);
        navigate(`${window.location.pathname}?${newQueryParams.toString()}`);

        setIsLoading(false);
    };

    const handleClearInput = () => {
        setValue("");
        setSuggestions([]);
    };

    return <>
        <Box>
            <Box sx={{ height: 'fit-content', maxHeight: 'calc(2 * 40px)', display: "flex", overflowX: "auto", maxWidth: "100%" }}>
                {loading ?
                    <>
                        <Skeleton variant="rounded" width="100%" height="32px" sx={{ mb: 1 }} />
                    </>
                    :
                    garageServiceTypes!.map(service =>
                        <Chip
                            key={service}
                            label={t(`${GarageServiceType[service]}.Filter`)}
                            variant={filters.includes(String(service)) ? "filled" : "outlined"}
                            sx={{ mr: 1, mb: 1 }}
                            onClick={() => handleChipClick(String(service))}
                        />
                    )
                }
            </Box>
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
                placeholder={t("Search by garage name...")}
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



