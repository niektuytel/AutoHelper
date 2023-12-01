import React from 'react';
import { Controller } from 'react-hook-form';
import { Box, Grid, TextField } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { getFormatedLicense, getLicenseFromPath } from '../../../../app/LicensePlateUtils';


interface IProps { }

export default ({ }: IProps) => {
    const { t } = useTranslation();

    return <>
        <Box flexGrow={1} p={1}>
        </Box>
    </>
};
