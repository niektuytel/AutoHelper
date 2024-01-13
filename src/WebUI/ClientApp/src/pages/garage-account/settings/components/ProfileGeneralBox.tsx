import React, { Dispatch, SetStateAction, useState } from 'react';
import { TextField, Box, InputAdornment, IconButton, Typography, Paper, Grid, List, ListItem } from "@mui/material";
import ClearIcon from '@mui/icons-material/Clear';
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import { useTranslation } from "react-i18next";
import { Controller, FieldErrors, FieldValues, UseFormSetValue, UseFormWatch } from 'react-hook-form';
import usePlacesAutocomplete, { getGeocode, getLatLng } from 'use-places-autocomplete';
import { useDispatch } from 'react-redux';


// custom imports
import { showOnError } from '../../../../redux/slices/statusSnackbarSlice';
import SearchGarageLookup from './SearchGarageLookup';
import { GarageLookupDtoItem } from '../../../../app/web-api-client';

interface LocationSectionProps {
    control: any;
    setFormValue: UseFormSetValue<FieldValues>;
    defaultLocation: any;
    notFound: boolean;
}

export default ({ control, setFormValue, defaultLocation, notFound }: LocationSectionProps) => {
    const [previousValue, setPreviousValue] = useState<any | null>(null);
    const { t } = useTranslation();
    const dispatch = useDispatch();
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
                country: 'nl',
            }
        },
        debounce: 250
    });

    const handleSearchByAddress = (address: string, city: string) => {
        getGeocode({ address: `${address}, ${city}` })
            .then(results => {
                const { lat, lng } = getLatLng(results[0]);

                // Set the values to useForm directly
                var newAddress = `${address}, ${city}`;
                setFormValue("address", newAddress);
                setFormValue("city", city);
                setFormValue("latitude", lat);
                setFormValue("longitude", lng);

                console.log("handleSearchByAddress", address, city, lat, lng);

                setPreviousValue({
                    address: address,
                    city: city,
                    latitude: lat,
                    longitude: lng
                });
            })
            .catch(error => {
                dispatch(showOnError(t("Error on getting address location")));
                console.error("Error fetching geocode:", error);
            });
    }

    const handleSearch = (place_id: string, address: string, city: string, postalCode: string) => {
        getGeocode({ placeId: place_id })
            .then(results => {
                const { lat, lng } = getLatLng(results[0]);

                // Set the values to useForm directly
                var newAddress = `${address}, ${city}`;
                setFormValue("address", newAddress);
                setFormValue("city", city);
                setFormValue("latitude", lat);
                setFormValue("longitude", lng);

                setPreviousValue({
                    address: address,
                    city: city,
                    latitude: lat,
                    longitude: lng,
                    postalCode: postalCode
                });
            })
            .catch(error => {
                dispatch(showOnError(t("Error on getting address location")));
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

        setFormValue("garageLookup", undefined);
    };

    const handleFocus = () => {
        setFormValue("address", "");
        setFormValue("city", "");
        setFormValue("latitude", "");
        setFormValue("longitude", "");
        setFormValue("postalCode", "");

        setValue("", false);
    };

    const handleBlur = () => {
        if (!value) {
            let address, city, latitude, longitude, postalCode;

            if (previousValue && previousValue.address && previousValue.city) {
                address = `${previousValue.address}, ${previousValue.city}`;
                city = previousValue.city;
                latitude = previousValue.latitude;
                longitude = previousValue.longitude;
                postalCode = previousValue.postalCode;
            } else if (defaultLocation && defaultLocation.location && defaultLocation.location.address && defaultLocation.location.city) {
                address = `${defaultLocation.location.address}, ${defaultLocation.location.city}`;
                city = defaultLocation.location.city;
                latitude = defaultLocation.location.latitude;
                longitude = defaultLocation.location.longitude;
                postalCode = defaultLocation.location.postalCode;
            }

            if (address && city) {
                setFormValue("address", address);
                setFormValue("city", city);
                setFormValue("latitude", latitude);
                setFormValue("longitude", longitude);
                setFormValue("postalCode", postalCode);

                setValue(address, false);
            }
        }
    };

    const garageLookupChanged = (value: GarageLookupDtoItem|undefined) => {
        setFormValue("garageLookup", value);
        if (!value) return;

        // General
        handleSearchByAddress(value.address!, value.city!);

        // Contact
        setFormValue("phoneNumber", value.phoneNumber);
        setFormValue("whatsappNumber", value.whatsappNumber);
        setFormValue("email", value.emailAddress);

        if (value?.conversationContactEmail) {
            setFormValue("conversationEmail", value?.conversationContactEmail);
        }
        else {
            setFormValue("conversationEmail", "");
        }

        if (value?.conversationContactWhatsappNumber) {
            setFormValue("conversationWhatsappNumber", value?.conversationContactWhatsappNumber);
        }
        else
        {
            setFormValue("conversationWhatsappNumber", "");
        }
    }

    return (
        <>
            <Grid item xs={12}>
                {notFound ? 
                    <Controller
                        name="garageLookup"
                        control={control}
                        rules={{ required: t('GarageAccount.GarageLookup.Requried') }}
                        render={({ field, fieldState: { error } }) => (
                            <SearchGarageLookup
                                value={field.value}
                                onChange={(value) => garageLookupChanged(value)}
                                error={error}
                            />
                        )}
                    />
                    :
                    <Controller
                        name="name"
                        control={control}
                        defaultValue=""
                        rules={{ required: t('GarageAccount.GarageLookup.Requried') }}
                        render={({ field, fieldState: { error } }) => (
                            <TextField
                                {...field}
                                fullWidth
                                size="small"
                                label={t("Name")}
                                variant="outlined"
                                error={!!error}
                                helperText={error ? t(error.message as string) : undefined}
                            />
                        )}
                    />
                }
            </Grid>
            <Grid item xs={12}>
                <Box position="relative">
                    <Controller
                        name="address"
                        control={control}
                        rules={{ required: t("Select an address") }}
                        defaultValue={""}
                        render={({ field, fieldState: { error } }) => (
                            <TextField
                                {...field}
                                onChange={(e) => {
                                    field.onChange(e);
                                    setValue(e.target.value)
                                }}
                                onFocus={handleFocus}
                                onBlur={handleBlur}
                                fullWidth
                                size="small"
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
                                error={!!error}
                                helperText={error ? t(error.message as string) : undefined}
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
        </>
    );
}

