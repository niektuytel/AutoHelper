import React, { useEffect, useState } from "react";
import { Box, Container, InputAdornment, TextField, IconButton, Button, Hidden, ListItem, List, useTheme, useMediaQuery, Autocomplete, CircularProgress, Chip, Skeleton } from "@mui/material";
import { Paper, Typography, Grid, ButtonBase } from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate } from "react-router-dom";

// own imports
import { GarageLookupDtoItem, GarageClient, GarageServiceType, PaginatedListOfGarageLookupBriefDto } from "../../../app/web-api-client";
import useGarageServiceTypes from "../../garage-account/servicelogs/useGarageServiceTypes";



interface IProps
{
    license_plate: string,
    latitude: number,
    longitude: number,
    in_km_range: number,
    page_size: number,
    onSearchExecuted: (data: PaginatedListOfGarageLookupBriefDto) => void;
}

export default ({ license_plate, latitude, longitude, in_km_range, page_size, onSearchExecuted }: IProps) => {
    const navigate = useNavigate();
    const theme = useTheme();
    const location = useLocation();
    const queryParams = new URLSearchParams(window.location.search);
    const { t } = useTranslation(["translations", "serviceTypes"]);

    const [isFocused, setIsFocused] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [value, setValue] = useState("");
    const [filters, setFilters] = useState<string[]>([]);
    const [suggestions, setSuggestions] = React.useState<readonly GarageLookupDtoItem[]>([]);

    //const { loading, garageServiceTypes } = useGarageServiceTypes(license_plate!);
    const useGarageClient = new GarageClient(process.env.PUBLIC_URL);

    useEffect(() => {
        if (queryParams.has("filters")) {
            const filters = queryParams.get("filters")?.split(",");
            setFilters(filters || []);
        }
        
    }, [window.location.search]);

    const fetchGaragesData = async (autocomplete: string, filterValues: string[]): Promise<PaginatedListOfGarageLookupBriefDto> => {
        try {
            const response = await useGarageClient.searchLookups(
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
        navigate(`${window.location.pathname}?${newQueryParams.toString()}`, { state: { from: location } });

        setIsLoading(false);
    };

    const handleClearInput = () => {
        setValue("");
        setSuggestions([]);
    };

    return <>
        <Box>
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
            <Box sx={{ height: 'fit-content', maxHeight: 'calc(2 * 40px)', display: "flex", overflowX: "auto", maxWidth: "100%" }}>
                {/*{loading &&*/}
                    <>
                        <Skeleton variant="rounded" width="100%" height="32px" sx={{ mt: 1 }} />
                    </>
                    {/*// TODO: Uncomment this when the garage service types are ready*/}
                    {/*//:*/}
                    {/*//garageServiceTypes!.map(service =>*/}
                    {/*//    <Chip*/}
                    {/*//        key={service.id}*/}
                    {/*//        label={t(`serviceTypes:${GarageServiceType[service]}.Filter`)}*/}
                    {/*//        variant={filters.includes(String(service)) ? "filled" : "outlined"}*/}
                    {/*//        sx={{ mr: 1, mt: 1 }}*/}
                    {/*//        onClick={() => handleChipClick(String(service))}*/}
                    {/*//    />*/}
                    {/*//)*/}
                {/*}*/}
            </Box>
        </Box>
    </>
}



