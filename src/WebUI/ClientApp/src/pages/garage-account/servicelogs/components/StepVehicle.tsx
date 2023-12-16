import React, { ChangeEvent, useState } from 'react';
import { Controller } from 'react-hook-form';
import { Box, FormControl, InputLabel, Select, MenuItem, TextField, Chip, Button, Grid, CircularProgress } from '@mui/material';
import { useTranslation } from 'react-i18next';
import AttachFileIcon from '@mui/icons-material/AttachFile';

// own imports
import useGarageServiceTypes from '../useGarageServiceTypes';
import { getFormatedLicense } from '../../../../app/LicensePlateUtils';
import { GarageServiceDtoItem } from '../../../../app/web-api-client';

interface IProps {
    licensePlate: string;
    setSelectedService: (service: GarageServiceDtoItem | undefined) => void;
    control: any;
    file: File | null;
    setFile: (file: File | null) => void;
}

const GarageStep = ({ licensePlate, setSelectedService, control, file, setFile }: IProps) => {
    const { t } = useTranslation();
    const { loading, isError, garageServiceTypes, triggerFetch } = useGarageServiceTypes(licensePlate);

    const getOptionsWithCurrentTitle = (currentTitle: string) => {
        if (!garageServiceTypes) {
            if (currentTitle) {
                return [{ id: 'temp-id', title: currentTitle }];
            }

            return [];
        }

        // Check if currentTitle is in garageServiceTypes
        const titleExists = garageServiceTypes?.some(service => service.title === currentTitle);

        // If the currentTitle is not in garageServiceTypes, add it
        if (!titleExists && currentTitle) {
            return [{ id: 'temp-id', title: currentTitle }, ...garageServiceTypes];
        }

        return garageServiceTypes;
    };


    const handleServiceChange = (event: any) => {
        if (!garageServiceTypes) return;

        const selectedService = garageServiceTypes!.find(service => service.title === event.target.value);
        if (selectedService) {
            setSelectedService(selectedService);
        }
    };

    const handleFileChange = (event: ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files ? event.target.files[0] : null;
        setFile(file);
    };

    const removeFileChange = () => {
        setFile(null);
    };

    const handleLicensePlateChange = (e: any): string => {
        let license = e.target.value.toUpperCase().replace(/-/g, '');
        license = getFormatedLicense(license);
        return license;
    };

    return <>
        <Box flexGrow={1} p={1}>
            <Grid item xs={12}>
                <Controller
                    name="licensePlate"
                    control={control}
                    defaultValue={""}
                    rules={{ required: t("AddMaintenanceLog.LicensePlate.Required") }}
                    render={({ field, fieldState: { error } }) => (
                        <TextField
                            {...field}
                            value={field.value}
                            onChange={(value) => {
                                const license = handleLicensePlateChange(value);

                                field.onChange(license);
                                triggerFetch(license);
                            }}
                            label={t("AddMaintenanceLog.LicensePlate.Label")}
                            fullWidth
                            size='small'
                            sx={{ mb: 1 }}
                            error={!!error}
                            helperText={error ? error.message : null}
                        />
                    )}
                />
            </Grid>
            <Controller
                name="title"
                control={control}
                defaultValue={""}
                render={({ field, fieldState: { error } }) => (
                    <FormControl fullWidth sx={{ mb: 2, mt: 2 }} size='small'>
                        <InputLabel id="service-type-label">
                            {t("AddMaintenanceLog.ServiceType.Label")}
                        </InputLabel>
                        <Select
                            {...field}
                            value={field.value}
                            onChange={(e) => {
                                field.onChange(e);
                                handleServiceChange(e);
                            }}
                            labelId="service-type-label"
                            label={t("AddMaintenanceLog.ServiceType.Label")}
                            endAdornment={loading ? <CircularProgress size={24} /> : null}
                        >
                            {getOptionsWithCurrentTitle(field.value).map((service) => (
                                <MenuItem key={service.id} value={service.title}>
                                    {service.title}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                )}
            />
            <Controller
                name="description"
                control={control}
                render={({ field, fieldState: { error } }) => (
                    <TextField
                        {...field}
                        label={t("AddMaintenanceLog.ServiceDescription.Label")}
                        multiline
                        rows={4}
                        fullWidth
                        sx={{ mb: 1 }}
                        error={!!error}
                        helperText={error ? error.message : null}
                    />
                )}
            />
            <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
                {file?.name !== null && file?.name !== undefined ?
                    <Chip label={file!.name} variant="outlined" onDelete={removeFileChange} /> : null}
                <Button
                    component="label"
                    variant="outlined"
                    startIcon={<AttachFileIcon />}
                    sx={{ color: "gray", borderColor: "gray" }}
                >
                    {t("AddMaintenanceLog.Attachments.Label")}
                    <input
                        type="file"
                        hidden
                        onChange={handleFileChange}
                        accept="image/*,application/pdf"
                    />
                </Button>
            </div>
        </Box>
    </>
};

export default GarageStep;
