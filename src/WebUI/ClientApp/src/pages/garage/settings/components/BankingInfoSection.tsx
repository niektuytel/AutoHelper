import React, { useState } from 'react';
import { TextField, Box, Typography } from '@mui/material';
import { BankingInfoItem } from '../../../../app/web-api-client';

interface BankingInfoProps {
    bankingInfo?: BankingInfoItem;
    onChange?: (location: BankingInfoItem) => void;
}

const BankingInfoSection: React.FC<BankingInfoProps> = ({ bankingInfo, onChange }) => {
    const [currentBankingInfo, setCurrentBankingInfo] = useState<BankingInfoItem>(bankingInfo || new BankingInfoItem());

    type BankingInfoItemProperties = Pick<BankingInfoItem, "bankName" | "accountNumber" | "iban" | "swiftCode">;

    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = event.target;
        const key = name as keyof BankingInfoItemProperties;
        const updatedBankingInfo: BankingInfoItem = currentBankingInfo;
        updatedBankingInfo[key] = value;

        // set location
        setCurrentBankingInfo(updatedBankingInfo);
        onChange && onChange(updatedBankingInfo);
    };

    return (
        <Box>
            <Typography variant="h5" gutterBottom>
                BankingInfo Details
            </Typography>
            <Box marginBottom={2}>
                <TextField
                    fullWidth
                    label="BankName"
                    variant="outlined"
                    name="bankInfo"
                    value={currentBankingInfo.bankName}
                    onChange={handleInputChange}
                    type="string"
                />
            </Box>
            <Box marginBottom={2}>
                <TextField
                    fullWidth
                    label="AccountNumber"
                    variant="outlined"
                    name="accountNumber"
                    value={currentBankingInfo.accountNumber}
                    onChange={handleInputChange}
                    type="string"
                />
            </Box>
            <Box marginBottom={2}>
                <TextField
                    fullWidth
                    label="IBAN"
                    variant="outlined"
                    name="iban"
                    value={currentBankingInfo.iban}
                    onChange={handleInputChange}
                    type="string"
                />
            </Box>
            <Box marginBottom={2}>
                <TextField
                    fullWidth
                    label="SwiftCode"
                    variant="outlined"
                    name="swiftCode"
                    value={currentBankingInfo.swiftCode}
                    onChange={handleInputChange}
                    type="string"
                />
            </Box>
        </Box>
    );
}

export default BankingInfoSection;
