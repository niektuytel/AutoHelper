import React, { Dispatch, SetStateAction } from 'react';
import { TextField, Box, InputAdornment, IconButton, Typography, Paper, Grid, List, ListItem } from "@mui/material";
import ClearIcon from '@mui/icons-material/Clear';
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import { useTranslation } from "react-i18next";
import { Controller, FieldErrors, FieldValues, UseFormSetValue, UseFormWatch } from 'react-hook-form';
import usePlacesAutocomplete, { getGeocode, getLatLng } from 'use-places-autocomplete';
import { GarageSettings, LocationItem } from '../../../../app/web-api-client';

interface LocationSectionProps {
    control: any;
    errors: FieldErrors<FieldValues>;
    setFormValue: UseFormSetValue<FieldValues>;
}

export default ({ control, errors, setFormValue }: LocationSectionProps) => {
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

    const handleSearch = (place_id: string, address: string, city: string, postalCode: string) => {
        getGeocode({ placeId: place_id })
            .then(results => {
                const { lat, lng } = getLatLng(results[0]);

                // Set the values to useForm directly
                setFormValue("address", `${address}, ${city}`);
                setFormValue("city", city);
                setFormValue("latitude", lat);
                setFormValue("longitude", lng);

                if (postalCode.length != 0) {
                    setFormValue("postalCode", postalCode);
                }
            })
            .catch(error => {
                // TODO: trigger an snackbar on redux, dispatch state with this error message (get it from the useTranslation)
                console.log("Error fetching geocode:", error);
            });
    }

    const handleSelect = (place_id: any, address: string, city: string, postalCode: string) => {
        // When user selects a place, we can replace the keyword without request data from API
        // by setting the second parameter to "false"
        setValue(`${address}, ${city}`, false);
        clearSuggestions();

        handleSearch(place_id, address, city, postalCode)
    };

    const handleClearInput = () => {
        // When user selects a place, we can replace the keyword without request data from API
        // by setting the second parameter to "false"
        setValue("", false);
        clearSuggestions();
    };

    return (
        <>
            <Grid item xs={12}>
                <Controller
                    name="name"
                    control={control}
                    rules={{ required: t("Name is required!") }}
                    defaultValue={""}
                    render={({ field }) => (
                        <TextField
                            {...field}
                            fullWidth
                            label={t("Name")}
                            variant="outlined"
                            error={Boolean(errors.name)}
                            helperText={errors.name ? t(errors.name.message as string) : undefined}
                        />
                    )}
                />
            </Grid>
            <Grid item xs={12} sm={6}>
                <Box position="relative">
                    <Controller
                        name="address"
                        control={control}
                        rules={{ required: "Address is required!" }}
                        defaultValue={""}
                        render={({ field }) => (
                            <TextField
                                {...field}
                                //value={value || ''}
                                onChange={(e) => {
                                    field.onChange(e);
                                    setValue(e.target.value)
                                }}
                                fullWidth
                                autoComplete="new-password"
                                autoFocus={true}
                                disabled={!ready}
                                variant="outlined"
                                label={t("search_places_placeholder")}
                                placeholder={t("search_places_placeholder")}
                                InputProps={{
                                    endAdornment: (
                                        <InputAdornment position="end">
                                            {value.length > 0 &&
                                                <IconButton onClick={handleClearInput}>
                                                    <ClearIcon color="action" />
                                                </IconButton>
                                            }
                                        </InputAdornment>
                                    ),
                                }}
                                error={Boolean(errors.address)}
                                helperText={ errors.address ? (errors.address.message as string) : undefined }
                            />
                        )}
                    />
                    {status === "OK" && ready &&
                        <Box position="absolute" top="60px" width="100%" zIndex={2} mt="5px">
                            <Paper elevation={3}>
                                <List dense disablePadding>
                                    {data.filter((x: any) => (x.terms.length > 2 && x.types.includes("premise") && x.types.includes("geocode"))).map((suggestion: any) => {
                                        const { place_id, terms } = suggestion;
                                        const address = `${terms[0].value} ${terms[1].value}`;
                                        const city = terms[terms.length - 2].value;
                                        const postalCodeVal = terms.length === 5 ? terms[terms.length - 3].value : "";
                                        return (
                                            <ListItem
                                                button
                                                key={place_id}
                                                onClick={() => handleSelect(place_id, address, city, postalCodeVal)}
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
                                                            {`${terms[0].value} ${terms[1].value}, ${terms[2].value}`}
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
                </Box>
            </Grid>
            <Grid item xs={12} sm={6}>
                <Controller
                    name="postalCode"
                    control={control}
                    rules={{ required: "Postal Code is required!" }}
                    defaultValue={""}
                    render={({ field }) => (
                        <TextField
                            {...field}
                            fullWidth
                            label="Postal Code"
                            variant="outlined"
                            error={Boolean(errors.postalCode)}
                            helperText={errors.postalCode ? (errors.postalCode.message as string) : undefined}
                        />
                    )}
                />
            </Grid>
        </>
    );
}

