import React, { useEffect, useState, memo } from 'react';
import { FieldError } from 'react-hook-form';
import { Autocomplete, TextField, CircularProgress } from '@mui/material';
import { GarageClient, GarageLookupSimplefiedDto } from '../../../app/web-api-client';

interface ISearchGarageProps {
    value: string,
    onChange: (value: string) => void,
    error?: FieldError
}

export default memo(({ value, onChange, error }: ISearchGarageProps) => {
    const [options, setOptions] = useState<GarageLookupSimplefiedDto[]>([]);
    const garageClient = new GarageClient(process.env.PUBLIC_URL);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        const fetchOptions = async () => {
            if (!value) return; // Exit early if value is empty
            setLoading(true);
            try {
                const response = await garageClient.searchLookupsByName(value, 5);
                setOptions(response);
            } catch (error) {
                console.error('Error fetching data:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchOptions();
    }, [value]);

    return (
        <Autocomplete
            freeSolo
            options={options}
            value={value}
            getOptionLabel={(option) =>
                typeof option === 'string' ? option : option.name || ''
            }
            onInputChange={(event, newInputValue) => {
                onChange(newInputValue);
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


