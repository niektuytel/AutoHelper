import React from 'react';
import { Controller } from 'react-hook-form';
import { Box, TextField } from '@mui/material';
import { useTranslation } from 'react-i18next';

interface IProps {
    control: any;
}

const ConfirmationStep = ({ control }: IProps) => {
    const { t } = useTranslation();

    return <>
        <Box flexGrow={1} p={1}>
            <Controller
                name="createdby"
                control={control}
                rules={{ required: t('AddMaintenanceLog.ServiceCreatedBy.Required') }}
                render={({ field, fieldState: { error } }) => (
                    <TextField
                        {...field}
                        sx={{ mb: 2 }}
                        value={field.value || ''}
                        label={t('AddMaintenanceLog.ServiceCreatedBy.Label')}
                        fullWidth
                        size='small'
                        error={!!error}
                        helperText={error ? error.message : null}
                    />
                )}
            />
            <Controller
                name="phonenumber"
                control={control}
                render={({ field, fieldState: { error } }) => (
                    <TextField
                        {...field}
                        sx={{ mb: 2 }}
                        value={field.value || ''}
                        label={t('AddMaintenanceLog.ServicePhoneNumber.Label')}
                        fullWidth
                        type="tel"
                        size='small'
                        error={!!error}
                        helperText={error ? error.message : null}
                    />
                )}
            />
            <Controller
                name="emailaddress"
                control={control}
                render={({ field, fieldState: { error } }) => (
                    <TextField
                        {...field}
                        sx={{ mb: 2 }}
                        value={field.value || ''}
                        label={t('AddMaintenanceLog.ServiceEmailAddress.Label')}
                        fullWidth
                        type="email"
                        size='small'
                        error={!!error}
                        helperText={error ? error.message : null}
                    />
                )}
            />
        </Box>
    </>
};

export default ConfirmationStep;
