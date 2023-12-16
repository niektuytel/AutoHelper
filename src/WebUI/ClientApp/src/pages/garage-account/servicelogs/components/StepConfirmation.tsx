import React from 'react';
import { Controller } from 'react-hook-form';
import { Box, Grid, TextField } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { getFormatedLicense, getLicenseFromPath } from '../../../../app/LicensePlateUtils';

interface IProps {
    mode: 'create' | 'edit';
    expectedNextDate: boolean;
    expectedNextOdometerReading: boolean;
    control: any;
}

const ConfirmationStep = ({ mode, expectedNextDate, expectedNextOdometerReading, control }: IProps) => {
    const { t } = useTranslation();

    return <>
        <Box flexGrow={1} p={1}>
            <Grid container spacing={2} sx={{ mb: 1 }}>
                <Grid item xs={(mode === 'edit' || expectedNextDate) ? 6 : 12}>
                    <Controller
                        name="date"
                        control={control}
                        defaultValue={new Date()}
                        rules={{ required: t("AddMaintenanceLog.Date.Required") }}
                        render={({ field, fieldState: { error } }) => (
                            <DatePicker
                                {...field}
                                label={t("AddMaintenanceLog.Date.Label")}
                                slotProps={{
                                    textField: {
                                        fullWidth: true,
                                        size: 'small',
                                        error: !!error,
                                        helperText: error ? error.message : null
                                    }
                                }}
                                format="dd/MM/yyyy"
                            />
                        )}
                    />
                </Grid>
                {(mode === 'edit' || expectedNextDate) &&
                    <Grid item xs={6}>
                        <Controller
                            name="expectedNextDate"
                            control={control}
                            rules={(mode === 'create') ? { required: t('AddMaintenanceLog.ExpectedNextDate.Required') } : {}}
                            render={({ field, fieldState: { error } }) => (
                                <DatePicker
                                    {...field}
                                    label={t('AddMaintenanceLog.ExpectedNextDate.Label')}
                                    slotProps={{
                                        textField: {
                                            fullWidth: true,
                                            size: 'small',
                                            error: !!error,
                                            helperText: error ? error.message : null
                                        }
                                    }}
                                    format="dd/MM/yyyy"
                                />
                            )}
                        />
                    </Grid>
                }
            </Grid>
            <Grid container spacing={2} sx={{ mb: 3 }}>
                <Grid item xs={(mode === 'edit' || expectedNextOdometerReading) ? 6 : 12}>
                    <Controller
                        name="odometerReading"
                        control={control}
                        rules={{ required: t('AddMaintenanceLog.OdometerReading.Required') }}
                        render={({ field, fieldState: { error } }) => (
                            <TextField
                                {...field}
                                value={field.value || ''}
                                label={t('AddMaintenanceLog.OdometerReading.Label')}
                                fullWidth
                                type="number"
                                size='small'
                                error={!!error}
                                helperText={error ? error.message : null}
                            />
                        )}
                    />
                </Grid>
                {(mode === 'edit' || expectedNextOdometerReading) &&
                    <Grid item xs={6}>
                        <Controller
                            name="expectedNextOdometerReading"
                            control={control}
                            rules={(mode === 'create') ? { required: t('AddMaintenanceLog.ExpectedNextOdometerReading.Required') } : {}}
                            render={({ field, fieldState: { error } }) =>
                                <TextField
                                    {...field}
                                    value={field.value || ''}
                                    label={t('AddMaintenanceLog.ExpectedNextOdometerReading.Label')}
                                    type="number"
                                    fullWidth
                                    size='small'
                                    error={!!error}
                                    helperText={error ? error.message : null}
                                />
                            }
                        />
                    </Grid>
                }
            </Grid>
        </Box>
    </>
};

export default ConfirmationStep;
