import React, { ChangeEvent, useEffect, useState } from 'react';
import { useForm, Controller, FieldError } from 'react-hook-form';
import CloseIcon from '@mui/icons-material/Close';
import {
    Autocomplete, TextField, Select, MenuItem, InputLabel, FormControl,
    Grid, Button, Divider, Typography, Box, IconButton, Drawer, styled, SvgIcon, Chip, useMediaQuery, useTheme, Stepper, StepLabel, Step
} from '@mui/material';
import { useDebounce } from 'use-debounce';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import ArrowForwardIcon from '@mui/icons-material/ArrowForward';
import { useTranslation } from 'react-i18next';
import AttachFileIcon from '@mui/icons-material/AttachFile';
import { GarageClient, GarageLookupSimplefiedDto, GarageServiceType, VehicleClient } from '../../../app/web-api-client';
import { enumToKeyValueArray } from '../../../app/utils';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { TimePickerProps } from '@mui/x-date-pickers';
import SearchGarage from './SearchGarage';

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


interface IServiceLogFormProps {
    licensePlate: string;
    drawerOpen: boolean;
    toggleDrawer: (open: boolean) => (event: React.KeyboardEvent | React.MouseEvent) => void;
}

interface IServiceLogFormData {
    performedByGarageName: string;
    type: string;
    description: string;
    date: Date | null;
    expectedNextDate: Date | null;
    odometerReading: number | '';
    expectedNextOdometerReading: number | '';
    createdby: string;
    phonenumber: string | null;
    emailaddress: string | null;
}

const steps = ['Garage', 'Vehicle', 'Confirmation']; // The steps in your process

export default ({ licensePlate, drawerOpen, toggleDrawer }: IServiceLogFormProps) => {
    const { t } = useTranslation(["translations", "serviceTypes"]);
    const [selectedType, setSelectedType] = useState<string>('');
    const [isMaintenance, setIsMaintenance] = useState<boolean>(false);
    const [file, setFile] = useState<File | null>(null);
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const { control, handleSubmit, formState: { errors }, reset } = useForm<IServiceLogFormData>();

    const [activeStep, setActiveStep] = useState(0);
    const [userInput, setUserInput] = useState(''); // For storing additional user input for step 2

    const handleFileChange = (event: ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files ? event.target.files[0] : null;
        setFile(file);
    };

    const removeFileChange = () => {
        setFile(null);
    };

    const handleNext = (data: IServiceLogFormData) => {
        if (activeStep === (steps.length - 1)) {
            onSubmit(data);
        }
        else {
            setActiveStep(activeStep+1);
        }
    };

    const handleBack = () => {
        setActiveStep((prevActiveStep) => prevActiveStep - 1);
    };

    const onSubmit = (data: any) => {
        console.log(data);

        if (file) {
            let attachmentFile = {
                data: file, // Your file object
                fileName: file?.name || ''
            };

            // Create the service log
            const vehicleClient = new VehicleClient(process.env.PUBLIC_URL);
            const response = vehicleClient.createServiceLog(
                licensePlate,
                data.garageIdentifier,
                data.type,
                data.description,
                data.date,
                data.expectedNextDate,
                data.odometerReading,
                data.expectedNextOdometerReading,
                data.createdby,
                data.phonenumber,
                data.emailaddress,
                file?.name || '',
                null, // Assuming FileData is handled separately
                attachmentFile
            ).then(response => {
                console.log(response);
            })

        } else {
            // Create the service log
            const vehicleClient = new VehicleClient(process.env.PUBLIC_URL);
            const response = vehicleClient.createServiceLog(
                licensePlate,
                data.performedByGarageName,
                data.type,
                data.description,
                data.date,
                data.expectedNextDate,
                data.odometerReading,
                data.expectedNextOdometerReading,
                data.createdby,
                data.phonenumber,
                data.emailaddress,
                null,
                null,
                null
            ).then(response => {
                console.log(response);
            })

        }

    };

    // TODO: Add validation for file size and type (only images) 
    // TODO: We need more information about the user (name, phone, email)
    console.log(errors);

    const drawerWidth = isMobile ? '100%' : '600px';
    return (
        <Drawer anchor="right" open={drawerOpen} onClose={toggleDrawer(false)} sx={{ width: drawerWidth }}>
            <Box sx={{ width: drawerWidth, display: 'flex', flexDirection: 'column', height: '100%' }} role="presentation">
                <Box display="flex" justifyContent="space-between" alignItems="center" p={1}>
                    <Typography variant="h6" component="div">
                        {t("AddMaintenanceLog.Title")}
                    </Typography>
                    <IconButton onClick={toggleDrawer(false)}>
                        <CloseIcon />
                    </IconButton>
                </Box>
                <Divider />
                <Stepper activeStep={activeStep} alternativeLabel sx={{ padding: theme.spacing(3) }}>
                    {steps.map((label) => (
                        <Step key={label}>
                            <StepLabel>{t(label)}</StepLabel>
                        </Step>
                    ))}
                </Stepper>
                {activeStep === 0 && (
                    <form onSubmit={handleSubmit(handleNext)} style={{ display: "contents" }}>
                        <Box flexGrow={1} p={1}>
                            <Controller
                                name="performedByGarageName"
                                control={control}
                                rules={{ required: 'Garage is required' }}
                                render={({ field, fieldState: { error } }) => (
                                    <SearchGarage
                                        value={field.value || ''}
                                        onChange={field.onChange}
                                        error={error}
                                    />
                                )}
                            />
                            <Controller
                                name="type"
                                control={control}
                                defaultValue={"Other"}
                                render={({ field }) => (
                                    <FormControl fullWidth sx={{ mb: 1, mt:1 }} size='small'>
                                        <InputLabel id="service-type-label">
                                            {t("AddMaintenanceLog.ServiceType.Label")}
                                        </InputLabel>
                                        <Select
                                            {...field}
                                            labelId="service-type-label"
                                            label={t("AddMaintenanceLog.ServiceType.Label")}
                                            onChange={(e) => {
                                                field.onChange(e);
                                                setSelectedType(e.target.value); 
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
                                render={({ field }) => (
                                    <TextField {...field} label={t("AddMaintenanceLog.ServiceDescription.Label")} multiline rows={4} fullWidth sx={{ mb: 1 }} />
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
                                    <input type="file" hidden onChange={handleFileChange} />
                                </Button>
                            </div>
                        </Box>
                        <Box p={1} component="footer" sx={{ mt: 1 }}>
                            <Button onClick={handleBack}>{t("Back")}</Button>
                            <Button variant="contained" color="primary" type="submit">
                                {t("Next")}
                            </Button>
                        </Box>
                    </form>
                )}
                {activeStep === 1 && (
                    <form onSubmit={handleSubmit(handleNext)} style={{ display: "contents" }}>
                        <Box flexGrow={1} p={1}>
                            <Grid container spacing={2} sx={{ mb: 1 }}>
                                <Grid item xs={isMaintenance ? 6 : 12}>
                                    <Controller
                                        name="date"
                                        control={control}
                                        defaultValue={new Date()}
                                        rules={{ required: t("AddMaintenanceLog.Date.Required") }}
                                        render={({ field, fieldState: { error } }) => (
                                            <DatePicker
                                                {...field}
                                                label={t("AddMaintenanceLog.Date.Label")}
                                                slotProps={{ textField: { fullWidth: true, size: 'small' } }}
                                                format="dd/MM/yyyy"
                                            />
                                        )}
                                    />
                                </Grid>
                                {isMaintenance &&
                                    <Grid item xs={6}>
                                        <Controller
                                            name="expectedNextDate"
                                            control={control}
                                            rules={{ required: t('AddMaintenanceLog.ExpectedNextDate.Required') }}
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
                                <Grid item xs={isMaintenance ? 6 : 12}>
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
                                {isMaintenance &&
                                    <Grid item xs={6}>
                                        <Controller
                                            name="expectedNextOdometerReading"
                                            control={control}
                                            rules={{ required: t('AddMaintenanceLog.ExpectedNextOdometerReading.Required') }}
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
                        <Box p={1} component="footer" sx={{ mt: 1 }}>
                            <Button onClick={handleBack}>{t("Back")}</Button>
                            <Button variant="contained" color="primary" type="submit">
                                {t("Next")}
                            </Button>
                        </Box>
                    </form>
                )}
                {activeStep === 2 && (
                    <form onSubmit={handleSubmit(handleNext)} style={{ display: "contents" }}>
                        <Box flexGrow={1} p={1}>
                            <Controller
                                name="createdby"
                                control={control}
                                render={({ field }) => (
                                    <TextField {...field} label={t("AddMaintenanceLog.ServiceCreatedBy.Label")} fullWidth sx={{ mb: 1 }} />
                                )}
                            />
                            <Controller
                                name="phonenumber"
                                control={control}
                                render={({ field }) => (
                                    <TextField {...field} label={t("AddMaintenanceLog.ServicePhoneNumber.Label")} fullWidth sx={{ mb: 1 }} />
                                )}
                            />
                            <Controller
                                name="emailaddress"
                                control={control}
                                render={({ field }) => (
                                    <TextField {...field} label={t("AddMaintenanceLog.ServiceEmailAddress.Label")} fullWidth sx={{ mb: 1 }} />
                                )}
                            />
                            <TextField
                                fullWidth
                                label={t("ServiceLog.ResponsiblePerson")}
                                value={userInput}
                                onChange={(e: ChangeEvent<HTMLInputElement>) => setUserInput(e.target.value)}
                                margin="normal"
                            />
                        </Box>
                        <Box p={1} component="footer" sx={{ mt: 1 }}>
                            <Button onClick={handleBack}>{t("Back")}</Button>
                            <Button variant="contained" color="primary" type="submit">
                                {t("Confirm")}
                            </Button>
                        </Box>
                    </form>
                )}
            </Box>
        </Drawer>
    );
};
