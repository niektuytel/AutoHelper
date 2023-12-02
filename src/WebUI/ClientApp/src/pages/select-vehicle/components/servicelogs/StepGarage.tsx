import React, { ChangeEvent } from 'react';
import { Controller } from 'react-hook-form';
import { Box, FormControl, InputLabel, Select, MenuItem, TextField, Chip, Button } from '@mui/material';
import { useTranslation } from 'react-i18next';
import AttachFileIcon from '@mui/icons-material/AttachFile';

import SearchGarage from './SearchGarage';
import { GarageServiceType } from '../../../../app/web-api-client';
import { enumToKeyValueArray } from '../../../../app/utils';

function getVehicleServicesTypes() {
    const items = [
        GarageServiceType.Other,
        GarageServiceType.Inspection,
        GarageServiceType.SmallMaintenance,
        GarageServiceType.GreatMaintenance,
        GarageServiceType.AirConditioningMaintenance,
        GarageServiceType.SeasonalTireChange
    ];

    return enumToKeyValueArray(GarageServiceType)
        .filter(({ key, value }) => items.includes(key))
        .map(({ key, value }) => ({
            key: key,
            value: value,
        }));
}

interface IProps {
    control: any;
    setIsMaintenance: (isMaintenance: boolean) => void;
    file: File | null;
    setFile: (file: File | null) => void;
}

const GarageStep = ({ control, setIsMaintenance, file, setFile }: IProps) => {
    const { t } = useTranslation();

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
                        onChange={(value) => field.onChange(value)}
                        error={error}
                    />
                )}
            />
            <Controller
                name="type"
                control={control}
                defaultValue={"Other"}
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
                                //setSelectedType(e.target.value);
                                setIsMaintenance(
                                    e.target.value === GarageServiceType[GarageServiceType.Inspection] ||
                                    e.target.value === GarageServiceType[GarageServiceType.SmallMaintenance] ||
                                    e.target.value === GarageServiceType[GarageServiceType.GreatMaintenance]
                                )
                            }}
                        >
                            {getVehicleServicesTypes().map(({ key, value }) => (
                                <MenuItem key={key} value={value}>
                                    {t(`serviceTypes:${value}.Title`)}
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
