import React, { useEffect, useState, useCallback, memo } from 'react';
import { FieldError } from 'react-hook-form';
import { Autocomplete, TextField, CircularProgress, debounce } from '@mui/material';

// own imports
import { GarageClient, GarageLookupSimplefiedDto } from '../../../../app/web-api-client';

interface ISearchGarageProps {
    value: GarageLookupSimplefiedDto;
    onChange: (value: GarageLookupSimplefiedDto) => void;
    error?: FieldError;
}

const fetchGarages = async (garageClient: GarageClient, search: string) => {
    if (!search) return [];
    try {
        const response = await garageClient.searchLookupCardsByName(search, 5);
        return response;
    } catch (error) {
        console.error('Error fetching data:', error);
        return [];
    }
};

const SearchGarage = memo(({ value, onChange, error }: ISearchGarageProps) => {
    const [options, setOptions] = useState<GarageLookupSimplefiedDto[]>([]);
    const garageClient = new GarageClient(process.env.PUBLIC_URL);
    const [search, setSearch] = useState('');
    const [loading, setLoading] = useState(false);

    const debouncedFetch = useCallback(debounce(async (searchValue: string) => {
        setLoading(true);
        const fetchedOptions = await fetchGarages(garageClient, searchValue);
        setOptions(fetchedOptions);
        setLoading(false);
    }, 300), []);

    useEffect(() => {
        debouncedFetch(search);
    }, [search, debouncedFetch]);

    return (
        <Autocomplete
            freeSolo
            options={options}
            value={value || ''}
            getOptionLabel={(option: any) => (option?.name && option?.city) ? `${option?.name}, ${option?.city}` : ''}
            onInputChange={(event, newInputValue) => {
                const garageLookup = options.find(option => `${option?.name}, ${option?.city}` === newInputValue);
                if (garageLookup) {
                    onChange(garageLookup);
                }

                setSearch(newInputValue);
            }}
            renderInput={(params) => (
                <TextField
                    {...params}
                    error={!!error}
                    helperText={error ? error.message : ''}
                    label="Garage"
                    size='small'
                    InputProps={{
                        ...params.InputProps,
                        endAdornment: (
                            <>
                                {loading ? <CircularProgress color="inherit" size={20} /> : null}
                                {params.InputProps.endAdornment}
                            </>
                        ),
                    }}
                />
            )}
        />
    );
});

export default SearchGarage;
