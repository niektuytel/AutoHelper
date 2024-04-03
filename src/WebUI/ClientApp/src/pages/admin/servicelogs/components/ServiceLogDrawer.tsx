import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import CloseIcon from '@mui/icons-material/Close';
import {
    Button, Divider, Typography, Box, IconButton, Drawer, useMediaQuery, useTheme, Stepper, StepLabel, Step, CircularProgress, Toolbar
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import CarIcon from '@mui/icons-material/DirectionsCar';
import PersonIcon from '@mui/icons-material/Person';
import CheckIcon from '@mui/icons-material/Check';
import { useDispatch } from 'react-redux';


// own imports
import StepVehicle from './StepVehicle';
import StepConfirmation from './StepConfirmation';
import { GarageServiceDtoItem, GarageServiceType, VehicleServiceLogAsGarageDtoItem, VehicleServiceLogStatus } from '../../../../app/web-api-client';
import { getFormatedLicense } from '../../../../utils/LicensePlateUtils';

interface IServiceLogDrawerData {
    id: string | undefined;
    licensePlate: string;
    garageServiceId: string | undefined;
    title: string;
    description: string;

    date: Date | null;
    expectedNextDate: Date | null;
    odometerReading: number | 0;
    expectedNextOdometerReading: number | 0;
    status?: VehicleServiceLogStatus;
}

interface IProps {
    mode: 'create' | 'edit';
    item: VehicleServiceLogAsGarageDtoItem | undefined;
    drawerOpen: boolean;
    toggleDrawer: (open: boolean) => void;
    handleService: (data: any, file: File | null) => void;
}

const steps = ['AddMaintenanceLog.Step.Vehicle.Title', 'AddMaintenanceLog.Step.Confirmation.Title'];

export default ({ mode, item, drawerOpen, toggleDrawer, handleService }: IProps) => {
    const { t } = useTranslation(["translations", "serviceTypes"]);
    const dispatch = useDispatch();
    const [selectedService, setVehicleService] = useState<GarageServiceDtoItem | undefined>(undefined);
    const [file, setFile] = useState<File | null>(null);
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const { control, handleSubmit, formState: { errors }, reset, setError, setValue } = useForm<IServiceLogDrawerData>();
    const [activeStep, setActiveStep] = useState(0);
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        if (mode === 'edit' && item) {
            setValue("id", item.id!);
            setValue("licensePlate", getFormatedLicense(item.vehicleLicensePlate!));
            setValue("title", item.title!);
            setValue("description", item.description!);
            setValue("date", item.date!);
            setValue("expectedNextDate", item.expectedNextDate!);
            setValue("odometerReading", item.odometerReading!);
            setValue("expectedNextOdometerReading", item.expectedNextOdometerReading!);
            setValue("status", item.status!);
        } else {
            reset();
        }
    }, [item, mode, setValue]);

    const handleNext = (data: IServiceLogDrawerData) => {
        let hasError = false;

        if (activeStep === 0 && selectedService) {
            // Validate Description for 'Other' Type
            if (selectedService.type === GarageServiceType.Other && !data.description?.trim()) {
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
                    setError('expectedNextOdometerReading', { type: 'manual', message: t('AddMaintenanceLog.NextOdometerReadingGTOdometerReading.Required')});
                    hasError = true;
                }
            }
        }

        if (hasError) {
            return;
        }

        if (activeStep === steps.length - 1) {
            data.garageServiceId = selectedService?.id || "";
            handleService(data, file);

            toggleDrawer(false);
            setActiveStep(0);
            reset();
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

    const drawerWidth = isMobile ? '100%' : '600px';
    return <Drawer
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
                {activeStep === 0 && <StepVehicle
                    licensePlate={item?.vehicleLicensePlate || ""}
                    setVehicleService={setVehicleService}
                    control={control}
                    file={file}
                    setFile={setFile}
                />}
                {activeStep === 1 && <StepConfirmation
                    mode={mode}
                    expectedNextDate={selectedService?.expectedNextDateIsRequired || false}
                    expectedNextOdometerReading={selectedService?.expectedNextOdometerReadingIsRequired || false}
                    control={control}
                />}
                <Box component="footer" sx={{ ml:1, mb: 2 }}>
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
    ;
};


const CustomStepIcon = ({ active, completed, icon }:any) => {
    const getIcon = () => {
        if (completed) {
            return <CheckIcon color='primary' />;
        }
        switch (icon) {
            case 1:
                return <CarIcon color='primary' />;
            case 2:
                return <PersonIcon color='primary' />;
            default:
                return <CheckIcon color='primary' />;
        }
    };

    return <div>{getIcon()}</div>;
};
