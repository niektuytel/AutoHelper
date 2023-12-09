import React, { ChangeEvent } from 'react';
import { Controller } from 'react-hook-form';
import { Box, FormControl, InputLabel, Select, MenuItem, TextField, Chip, Button, CircularProgress } from '@mui/material';
import { useTranslation } from 'react-i18next';
import AttachFileIcon from '@mui/icons-material/AttachFile';

import SearchGarage from './SearchGarage';
import { GarageServiceDtoItem, GarageServiceType } from '../../../../app/web-api-client';
import { enumToKeyValueArray } from '../../../../app/utils';
import useGarageServiceTypes from './useGarageServiceTypes';

interface IProps {
    control: any;
    licensePlate: string;
    setSelectedService: (service: GarageServiceDtoItem | undefined) => void;
    file: File | null;
    setFile: (file: File | null) => void;
}

const GarageStep = ({ control, licensePlate, setSelectedService, file, setFile }: IProps) => {
    const { t } = useTranslation();
    const { loading, isError, garageServiceTypes, triggerFetch } = useGarageServiceTypes(licensePlate);

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


    return <>
        <Box flexGrow={1} p={1}>
            <Controller
                name="garageLookup"
                control={control}
                rules={{ required: t('AddMaintenanceLog.GarageLookup.Requried') }}
                render={({ field, fieldState: { error } }) => (
                    <SearchGarage
                        value={field.value}
                        onChange={(value) => {
                            // remove selected service
                            setSelectedService(undefined);

                            field.onChange(value)
                            if (value.identifier) {
                                triggerFetch(value.identifier!);
                            }
                        }}
                        error={error}
                    />
                )}
            />
            <Controller
                name="garageService"
                control={control}
                defaultValue={""}
                render={({ field }) => (
                    <FormControl fullWidth sx={{ mb: 2, mt: 2 }} size='small'>
                        <InputLabel id="service-type-label">
                            {t("AddMaintenanceLog.ServiceType.Label")}
                        </InputLabel>
                        <Select
                            {...field}
                            labelId="service-type-label"
                            label={t("AddMaintenanceLog.ServiceType.Label")}
                            onChange={(e) => {
                                field.onChange(e);
                                handleServiceChange(e);
                            }}
                            endAdornment={loading ? <CircularProgress size={24} /> : null}
                        >
                            {garageServiceTypes?.map((service, index) => service.title &&
                                <MenuItem key={service.id} value={service.title}>
                                    {service.title}
                                </MenuItem>
                            )}
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
