import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useSearchParams } from "react-router-dom";
import { Box, InputAdornment, TextField, IconButton, CircularProgress, Chip } from "@mui/material";
import { Typography } from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';

// own imports
import { GarageLookupDtoItem, GarageServiceType } from "../../../app/web-api-client";


function useDebounce(value: any, delay: number) {
    const [debouncedValue, setDebouncedValue] = useState(value);

    useEffect(() => {
        const handler = setTimeout(() => {
            setDebouncedValue(value);
        }, delay);

        return () => {
            clearTimeout(handler);
        };
    }, [value, delay]);

    return debouncedValue;
}

interface IProps
{
    loading: boolean;
    items: GarageLookupDtoItem[];
}

export default ({ loading, items }: IProps) => {
    const { t } = useTranslation(["translations", "serviceTypes"]);
    const [searchParams, setSearchParams] = useSearchParams();
    const queryParams = new URLSearchParams(window.location.search);
    const [value, setValue] = useState(queryParams.get("input") || "");
    const [filters, setFilters] = useState<string[]>(queryParams.get("filters")?.split(",") || []);
    const [isFocused, setIsFocused] = useState(false);

    // Debounce for input value
    const debouncedValue = useDebounce(value, 500);

    useEffect(() => {
        // Update the URL
        setSearchParams({ input: value, filters: filters.join(",") });
    }, [debouncedValue, filters, setSearchParams]);

    const handleInput = async (e: any) => {
        setValue(e.target.value);
    }

    const handleChipClick = async (filterKey: string) => {
        var filterValues = filters;
        if (filters.includes(filterKey)) {
            filterValues = filterValues.filter(f => f !== filterKey);
        } else {
            filterValues = [...filterValues, filterKey];
        }

        setFilters(filterValues);
    };

    const handleClearInput = () => {
        setValue("");
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
                            {loading ? (
                                <CircularProgress size={20} sx={{ mr: 2 }} />
                            ) : (
                                <>
                                    {value.length > 0 && (items.length === 0 && isFocused) &&
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
                {[GarageServiceType.Repair, GarageServiceType.Service, GarageServiceType.Inspection].map((service, index) =>
                    <Chip
                        key={`service.id:${index}`}
                        label={t(`serviceTypes:${GarageServiceType[service]}.Filter`)}
                        variant={filters.includes(String(service)) ? "filled" : "outlined"}
                        sx={{ mr: 1, mt: 1 }}
                        onClick={() => handleChipClick(String(service))}
                    />
                )}
            </Box>
        </Box>
    </>
}



