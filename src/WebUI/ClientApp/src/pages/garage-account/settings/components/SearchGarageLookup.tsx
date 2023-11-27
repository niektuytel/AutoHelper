import React, { useEffect, useState, useCallback, memo } from 'react';
import { FieldError } from 'react-hook-form';
import { Autocomplete, TextField, CircularProgress, debounce } from '@mui/material';
import { GarageClient, GarageLookupDtoItem, GarageLookupSimplefiedDto } from '../../../../app/web-api-client';
import { useTranslation } from 'react-i18next';

// own imports

interface ISearchGarageProps {
    value: GarageLookupDtoItem | undefined;
    onChange: (value: GarageLookupDtoItem | undefined) => void;
    error?: FieldError;
}

const fetchGarages = async (garageClient: GarageClient, search: string) => {
    if (!search) return [];
    try {
        const response = await garageClient.searchLookupsByName(search, 5);
        return response;
    } catch (error) {
        console.error('Error fetching data:', error);
        return [];
    }
};

export default memo(({ value, onChange, error }: ISearchGarageProps) => {
    const [options, setOptions] = useState<GarageLookupSimplefiedDto[]>([]);
    const garageClient = new GarageClient(process.env.PUBLIC_URL);
    const [search, setSearch] = useState('');
    const [loading, setLoading] = useState(false);
    const { t } = useTranslation();

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
            getOptionLabel={(option: any) => option?.name || ''}
            onInputChange={(event, newInputValue) => {
                const garageLookup = options.find(option => option.name === newInputValue);
                onChange(garageLookup);

                setSearch(newInputValue);
            }}
            renderInput={(params) => (
                <TextField
                    {...params}
                    error={!!error}
                    helperText={error ? error.message : ''}
                    label={t("Name")}
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

