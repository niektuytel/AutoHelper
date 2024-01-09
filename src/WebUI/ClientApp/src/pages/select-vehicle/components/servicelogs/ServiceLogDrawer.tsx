import React, { useContext, useState } from 'react';
import { useForm } from 'react-hook-form';
import CloseIcon from '@mui/icons-material/Close';
import {
    Button, Divider, Typography, Box, IconButton, Drawer, useMediaQuery, useTheme, Stepper, StepLabel, Step, CircularProgress, Toolbar
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import GarageIcon from '@mui/icons-material/CarRepair';
import CarIcon from '@mui/icons-material/DirectionsCar';
import PersonIcon from '@mui/icons-material/Person';
import CheckIcon from '@mui/icons-material/Check';
import AddIcon from '@mui/icons-material/Add';

// own imports
import { BadRequestResponse, GarageLookupSimplefiedDto, GarageServiceDtoItem, GarageServiceType, VehicleClient, VehicleServiceLogDtoItem } from '../../../../app/web-api-client';
import StepConfirmation from './StepConfirmation';
import StepVehicle from './StepVehicle';
import StepGarage from './StepGarage';
import { showOnError, showOnSuccess } from '../../../../redux/slices/statusSnackbarSlice';
import { useDispatch } from 'react-redux';
import useVehicleServiceLogs from '../../useVehicleServiceLogs';
import useGarageServiceTypes from './useGarageServiceTypes';
import { ServiceLogDrawerContext } from '../../../../context/ServiceLogDrawerContext';

interface IServiceLogDrawerProps {
    licensePlate: string;
}

interface IServiceLogDrawerData {
    garageLookup: GarageLookupSimplefiedDto;
    garageService: GarageServiceDtoItem;
    title: string;
    description: string;
    date: Date | null;
    expectedNextDate: Date | null;
    odometerReading: number | 0;
    expectedNextOdometerReading: number | 0;
    createdby: string;
    phonenumber: string | null;
    emailaddress: string | null;
}

const steps = ['AddMaintenanceLog.Step.Garage.Title', 'AddMaintenanceLog.Step.Vehicle.Title', 'AddMaintenanceLog.Step.Confirmation.Title'];

export default ({ licensePlate }: IServiceLogDrawerProps) => {
    const { t } = useTranslation(["translations", "serviceTypes"]);
    const { addServiceLog } = useVehicleServiceLogs(licensePlate);
    const dispatch = useDispatch();
    const [selectedService, setSelectedService] = useState<GarageServiceDtoItem | undefined>(undefined);
    const [file, setFile] = useState<File | null>(null);
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const { control, handleSubmit, formState: { errors }, reset, setError, setValue } = useForm<IServiceLogDrawerData>();
    const { loading, isError, garageServiceTypes, triggerFetch } = useGarageServiceTypes(licensePlate);
    const [activeStep, setActiveStep] = useState(0);
    const [isLoading, setIsLoading] = useState(false);

    const context = useContext(ServiceLogDrawerContext);

    if (!context) {
        throw new Error("DrawerComponent must be used within a DrawerProvider");
    }

    const { drawerOpen, toggleDrawer } = context;

    const handleNext = (data: IServiceLogDrawerData) => {

        let hasError = false;

        if (activeStep === 0) {

            // Validate Garage
            if (!data.garageLookup) {
                setError('garageLookup', { type: 'manual', message: t('AddMaintenanceLog.Garage.Required') });
                hasError = true;
            }

            // Validate Garage Service
            if (!data.title) {
                setError('title', { type: 'manual', message: t('AddMaintenanceLog.ServiceType.Required') });
                hasError = true;
            }

            // Validate Description for 'Other' Type
            if (selectedService && selectedService.type === GarageServiceType.Other && !data.description?.trim()) {
                setError('description', { type: 'manual', message: t('AddMaintenanceLog.DescriptionOnTypeOther.Required')});
                hasError = true;
            }
        } else if (activeStep === 1) {
            // Validate Dates
            if (data.date && data.expectedNextDate) {
                const date = new Date(data.date);
                const expectedNextDate = new Date(data.expectedNextDate);

                if (isNaN(date.getTime()) || isNaN(expectedNextDate.getTime())) {
                    // Handle invalid date format
                    setError('expectedNextDate', { type: 'manual', message: t('Invalid date format') });
                    hasError = true;
                } else if (date > expectedNextDate) {
                    setError('expectedNextDate', { type: 'manual', message: t('AddMaintenanceLog.NextDateGTDate.Required') });
                    hasError = true;
                }
            }

            // Validate Odometer Readings
            if (data.odometerReading && data.expectedNextOdometerReading) {
                const odometerReading = new Number(data.odometerReading);
                const expectedNextOdometerReading = new Number(data.expectedNextOdometerReading);

                if (odometerReading > expectedNextOdometerReading) {
                    setError('expectedNextDate', { type: 'manual', message: t('AddMaintenanceLog.NextDateGTDate.Required') });
                    setError('expectedNextOdometerReading', { type: 'manual', message: t('AddMaintenanceLog.NextOdometerReadingGTOdometerReading.Required') });
                    hasError = true;
                }
            }
        } else if (activeStep === 2) {

            // Check if both phonenumber and emailaddress are not set
            if (!data.phonenumber?.trim() && !data.emailaddress?.trim()) {
                setError('phonenumber', { type: 'manual', message: t('AddMaintenanceLog.PhoneOrEmail.Required')});
                hasError = true;
            }
        }

        if (hasError) {
            return;
        }

        if (activeStep === steps.length - 1) {
            onSubmit(data);
        } else {
            setActiveStep(activeStep+1);
        }
    };

    const handleBack = () => {
        if (activeStep === 0)
        {
            toggleDrawer(false);
        }
        else
        {
            setActiveStep((prevActiveStep) => prevActiveStep - 1);
        }
    };

    const onSubmit = (data: any) => {
        console.log(data);
        setIsLoading(true); // Set loading to true

        const vehicleClient = new VehicleClient(process.env.PUBLIC_URL);
        const createLog = async () => {
            try {
                const response = await vehicleClient.createServiceLog(
                    licensePlate,
                    data.garageLookup.identifier,
                    selectedService!.id,
                    data.description,
                    data.date.toISOString(), 
                    data.expectedNextDate ? data.expectedNextDate.toISOString() : null, 
                    data.odometerReading,
                    data.expectedNextOdometerReading,
                    data.createdby,
                    data.phonenumber,
                    data.emailaddress,
                    file ? { data: file, fileName: file?.name || '' } : null
                );

                dispatch(showOnSuccess(t('AddMaintenanceLog.Succeeded')));

                // Reset only specific form fields
                setValue('description', '');
                setValue('date', null);
                setValue('expectedNextDate', null);
                setValue('odometerReading', 0);
                setValue('expectedNextOdometerReading', 0);

                addServiceLog(response);
                toggleDrawer(false);
                setActiveStep(0);
            } catch (error) {
                console.error('Error:', error);

                // Display specific error message from server response
                if (error instanceof BadRequestResponse && error.errors) {
                    dispatch(showOnError(Object.entries(error.errors)[0][1]));
                }
            } finally {
                setIsLoading(false); // Reset loading to false regardless of request outcome
            }
        };

        createLog();
    };

    const drawerWidth = isMobile ? '100%' : '600px';
    return <>
        <Drawer
            anchor="right"
            open={drawerOpen}
            onClose={() => toggleDrawer(false)}
            sx={{
                '& .MuiDrawer-paper': {
                    width: isMobile ? '100%' : '600px',
                },
            }}
        >
            <Toolbar
                sx={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: "space-between",
                    width: "100%",
                    padding: "12px!important",
                }}
            >
                <Typography variant="h6" component="div">
                    {t("AddMaintenanceLog.Title")}
                </Typography>
                <IconButton onClick={() => toggleDrawer(false)}>
                    <CloseIcon />
                </IconButton>
            </Toolbar>
            <Divider />
            <Box sx={{ width: drawerWidth, display: 'flex', flexDirection: 'column', height: '100%' }} role="presentation">
                <Stepper activeStep={activeStep} alternativeLabel sx={{ padding: theme.spacing(3) }}>
                    {steps.map((label, index) => (
                        <Step key={label} completed={activeStep > index}>
                            <StepLabel StepIconComponent={(props) => <CustomStepIcon {...props} />}>
                                {t(label)}
                            </StepLabel>
                        </Step>
                    ))}
                </Stepper>
                <form onSubmit={handleSubmit(handleNext)} style={{ display: "contents" }}>
                    {activeStep === 0 && <StepGarage
                        control={control}
                        setValue={setValue}
                        loading={loading}
                        garageServiceTypes={garageServiceTypes}
                        triggerFetch={triggerFetch}
                        selectedService={selectedService}
                        setSelectedService={setSelectedService}
                        file={file}
                        setFile={setFile}
                    />}
                    {activeStep === 1 && <StepVehicle
                        control={control}
                        expectedNextDate={selectedService?.expectedNextDateIsRequired || false}
                        expectedNextOdometerReading={selectedService?.expectedNextOdometerReadingIsRequired || false}
                    />}
                    {activeStep === 2 && <StepConfirmation control={control} />}
                    <Box component="footer" sx={{ ml: 1, mb: 1 }}>
                        <Button onClick={handleBack}>{(activeStep === 0) ? t("Cancel") : t("Back")}</Button>
                        <Button
                            variant="contained"
                            color="primary"
                            type="submit"
                            disabled={isLoading}
                        >
                            {isLoading ?
                                <CircularProgress size={24} />
                                :
                                (activeStep === steps.length - 1) ? t("Confirm") : t("Next")
                            }
                        </Button>
                    </Box>
                </form>
            </Box>
        </Drawer>
    </>;

    //<Button
    //    variant="contained"
    //    color="primary"
    //    sx={{
    //        position: 'fixed',
    //        p: 2,
    //        bottom: 16,
    //        right: 16,
    //        borderRadius: 10,
    //        zIndex: 1000
    //    }}
    //    onClick={() => toggleDrawer(true)}
    //    endIcon={<AddIcon />}
    //>
    //    {t("Maintenance")}
    //</Button>
};


const CustomStepIcon = ({ active, completed, icon }:any) => {
    const getIcon = () => {
        if (completed) {
            return <CheckIcon color='primary' />;
        }
        switch (icon) {
            case 1:
                return <GarageIcon color='primary' />;
            case 2:
                return <CarIcon color='primary' />;
            case 3:
                return <PersonIcon color='primary' />;
            default:
                return <CheckIcon color='primary' />;
        }
    };

    return <div>{getIcon()}</div>;
};
