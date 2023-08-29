import React, { useState } from 'react';
import { TextField, Box, Typography } from '@mui/material';
import { BusinessOwnerItem } from '../../../../app/web-api-client';

interface BusinessOwnerProps {
    businessOwner?: BusinessOwnerItem;
    onChange?: (location: BusinessOwnerItem) => void;
}

const BusinessOwnerSection: React.FC<BusinessOwnerProps> = ({ businessOwner, onChange }) => {
    const [currentBusinessOwner, setCurrentBusinessOwner] = useState<BusinessOwnerItem>(businessOwner || new BusinessOwnerItem());

    type BusinessOwnerItemProperties = Pick<BusinessOwnerItem, "fullName" | "phoneNumber" | "email">;

    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = event.target;
        const key = name as keyof BusinessOwnerItemProperties;
        const updatedBusinessOwner: BusinessOwnerItem = currentBusinessOwner;
        updatedBusinessOwner[key] = value;

        // set location
        setCurrentBusinessOwner(updatedBusinessOwner);
        onChange && onChange(updatedBusinessOwner);
    };

    return (
        <Box>
            <Typography variant="h5" gutterBottom>
                BusinessOwner Details
            </Typography>
            <Box marginBottom={2}>
                <TextField
                    fullWidth
                    label="FullName"
                    variant="outlined"
                    name="fullName"
                    value={currentBusinessOwner.fullName}
                    onChange={handleInputChange}
                    type="string"
                />
            </Box>
            <Box marginBottom={2}>
                <TextField
                    fullWidth
                    label="PhoneNumber"
                    variant="outlined"
                    name="phoneNumber"
                    value={currentBusinessOwner.phoneNumber}
                    onChange={handleInputChange}
                    type="string"
                />
            </Box>
            <Box marginBottom={2}>
                <TextField
                    fullWidth
                    label="Email"
                    variant="outlined"
                    name="email"
                    value={currentBusinessOwner.email}
                    onChange={handleInputChange}
                    type="email"
                />
            </Box>
        </Box>
    );
}

export default BusinessOwnerSection;
