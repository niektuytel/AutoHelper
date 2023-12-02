import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import CloseIcon from '@mui/icons-material/Close';
import {
    Button, Divider, Typography, Box, IconButton, Drawer, useMediaQuery, useTheme, Stepper, StepLabel, Step, CircularProgress
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import GarageIcon from '@mui/icons-material/CarRepair';
import CarIcon from '@mui/icons-material/DirectionsCar';
import PersonIcon from '@mui/icons-material/Person';
import CheckIcon from '@mui/icons-material/Check';


// own imports
import StepVehicle from './StepVehicle';
import StepGarage from './StepGarage';
import { useDispatch } from 'react-redux';
import { BadRequestResponse, GarageAccountClient, GarageLookupSimplefiedDto, VehicleClient, VehicleServiceLogAsGarageDtoItem, VehicleServiceLogDtoItem } from '../../../../app/web-api-client';
import { showOnError, showOnSuccess } from '../../../../redux/slices/statusSnackbarSlice';
import { GetGarageAccountClient } from '../../../../app/GarageClient';
import { useAuth0 } from '@auth0/auth0-react';
import { useDrawer } from '../../../select-vehicle/ServiceLogDrawerProvider';

interface IServiceLogFormProps {
    drawerOpen: boolean;
    toggleDrawer: (open: boolean) => void;
    handleService: (data: any, file: File | null) => void;
}

interface IServiceLogFormData {
    licensePlate: string;
    type: string;
    description: string;
    date: Date | null;
    expectedNextDate: Date | null;
    odometerReading: number | 0;
    expectedNextOdometerReading: number | 0;
    createdby: string;
    phonenumber: string | null;
    emailaddress: string | null;
}

const steps = ['AddMaintenanceLog.Step.Garage.Title', 'AddMaintenanceLog.Step.Vehicle.Title'];

export default ({ drawerOpen, toggleDrawer, handleService }: IServiceLogFormProps) => {
    const { t } = useTranslation(["translations", "serviceTypes"]);
    const dispatch = useDispatch();
    const [isMaintenance, setIsMaintenance] = useState<boolean>(false);
    const [file, setFile] = useState<File | null>(null);
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

    const { getAccessTokenSilently } = useAuth0();
    const accessToken = getAccessTokenSilently();
    const garageClient = GetGarageAccountClient(accessToken);
    const { control, handleSubmit, formState: { errors }, reset, setError, setValue } = useForm<IServiceLogFormData>();
    const [activeStep, setActiveStep] = useState(0);
    const [isLoading, setIsLoading] = useState(false);

    const handleNext = (data: IServiceLogFormData) => {
        let hasError = false;

        if (activeStep === 0) {

            // Validate Description for 'Other' Type
            if (data.type === 'Other' && !data.description?.trim()) {
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
            handleService(data, file);
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


    //const onSubmit = (data: any) => {
    //    handleService(data, file);

    //    //console.log(data);
    //    //setIsLoading(true); // Set loading to true

    //    //const garageAccountClient = new GarageAccountClient(process.env.PUBLIC_URL);
    //    //const createLog = async () => {
    //    //    try {
    //    //        const response = await garageClient.createServiceLog(
    //    //            data.licensePlate,
    //    //            data.type,
    //    //            data.description,
    //    //            data.date.toISOString(),
    //    //            data.expectedNextDate ? data.expectedNextDate.toISOString() : null,
    //    //            data.odometerReading,
    //    //            data.expectedNextOdometerReading,
    //    //            file ? { data: file, fileName: file?.name || '' } : null
    //    //        );

    //    //        // Reset only specific form fields
    //    //        setValue('type', '');
    //    //        setValue('description', '');
    //    //        setValue('date', null);
    //    //        setValue('expectedNextDate', null);
    //    //        setValue('odometerReading', 0);
    //    //        setValue('expectedNextOdometerReading', 0);
    //    //        file && setFile(null);

    //    //        // done
    //    //        setActiveStep(0); // Reset active step to 0
    //    //        handleService(response, file);
    //    //        dispatch(showOnSuccess(t('AddMaintenanceLog.Succeeded')));
    //    //    } catch (error) {
    //    //        console.error('Error:', error);

    //    //        // Display specific error message from server response
    //    //        if (error instanceof BadRequestResponse && error.errors) {
    //    //            dispatch(showOnError(Object.entries(error.errors)[0][1]));
    //    //        }
    //    //    } finally {
    //    //        setIsLoading(false); // Reset loading to false regardless of request outcome
    //    //    }
    //    //};

    //    //createLog();
    //};

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
        <Box sx={{ width: drawerWidth, display: 'flex', flexDirection: 'column', height: '100%' }} role="presentation">
            <Box display="flex" justifyContent="space-between" alignItems="center" p={1}>
                <Typography variant="h6" component="div">
                    {t("AddMaintenanceLog.Title")}
                </Typography>
                <IconButton onClick={() => toggleDrawer(false)}>
                    <CloseIcon />
                </IconButton>
            </Box>
            <Divider />
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
                {activeStep === 0 && <StepGarage control={control} setIsMaintenance={setIsMaintenance} file={file} setFile={setFile} />}
                {activeStep === 1 && <StepVehicle control={control} isMaintenance={isMaintenance} />}
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
                return <GarageIcon color='primary' />;
            case 2:
                return <CarIcon color='primary' />;
            default:
                return <CheckIcon color='primary' />;
        }
    };

    return <div>{getIcon()}</div>;
};
