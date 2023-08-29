import React, { useState } from 'react';
import { TextField, Box, Typography } from '@mui/material';
import { LocationItem } from '../../../../app/web-api-client';

interface LocationProps {
    location?: LocationItem;
    onChange?: (location: LocationItem) => void;
}

const LocationSection: React.FC<LocationProps> = ({ location, onChange }) => {
    const [currentLocation, setCurrentLocation] = useState<LocationItem>(location || new LocationItem());

    type LocationItemProperties = Pick<LocationItem, "longitude" | "latitude" | "address" | "city" | "postalCode" | "country">;

    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = event.target;

        const key = name as keyof LocationItemProperties;

        // Create a copy of the existing object
        const updatedLocation: Partial<LocationItem> = { ...currentLocation };

        // Type guard for longitude and latitude
        if (key === "longitude" || key === "latitude") {
            updatedLocation[key] = parseFloat(value);
        } else {
            updatedLocation[key] = value;
        }

        // Update state. Note: This does rely on currentLocation being a LocationItem or you might lose other methods/properties.
        setCurrentLocation(prevState => ({ ...prevState, ...updatedLocation }));
        onChange && onChange({ ...new LocationItem(), ...updatedLocation });
    };

    return (
        <Box>
            <Typography variant="h5" gutterBottom>
                Location Details
            </Typography>
            <Box marginBottom={2}>
                <TextField
                    fullWidth
                    label="Longitude"
                    variant="outlined"
                    name="longitude"
                    value={currentLocation.longitude.toString()}
                    onChange={handleInputChange}
                    type="number"
                />
            </Box>
            <Box marginBottom={2}>
                <TextField
                    fullWidth
                    label="Latitude"
                    variant="outlined"
                    name="latitude"
                    value={currentLocation.latitude.toString()}
                    onChange={handleInputChange}
                    type="number"
                />
            </Box>
            {/* ... Rest of the fields are unchanged ... */}
        </Box>
    );
}

export default LocationSection;
