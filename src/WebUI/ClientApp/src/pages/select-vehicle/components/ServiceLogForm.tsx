import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import CloseIcon from '@mui/icons-material/Close';
import {
    Button, Divider, Typography, Box, IconButton, Drawer, useMediaQuery, useTheme, Stepper, StepLabel, Step, CircularProgress
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import GarageIcon from '@mui/icons-material/HomeWork';
import CarIcon from '@mui/icons-material/DirectionsCar';
import PersonIcon from '@mui/icons-material/Person';
import CheckIcon from '@mui/icons-material/Check';


// own imports
import { BadRequestResponse, GarageLookupSimplefiedDto, VehicleClient } from '../../../app/web-api-client';
import StepConfirmation from './StepConfirmation';
import StepVehicle from './StepVehicle';
import StepGarage from './StepGarage';
import { showOnError } from '../../../redux/slices/statusSnackbarSlice';
import { useDispatch } from 'react-redux';

interface IServiceLogFormProps {
    licensePlate: string;
    drawerOpen: boolean;
    toggleDrawer: (open: boolean) => (event: React.KeyboardEvent | React.MouseEvent) => void;
}

interface IServiceLogFormData {
    garageLookup: GarageLookupSimplefiedDto;
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

const steps = ['AddMaintenanceLog.Step.Garage.Title', 'AddMaintenanceLog.Step.Vehicle.Title', 'AddMaintenanceLog.Step.Confirmation.Title'];

export default ({ licensePlate, drawerOpen, toggleDrawer }: IServiceLogFormProps) => {
    const { t } = useTranslation(["translations", "serviceTypes"]);
    const dispatch = useDispatch();
    const [isMaintenance, setIsMaintenance] = useState<boolean>(false);
    const [file, setFile] = useState<File | null>(null);
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
    const { control, handleSubmit, formState: { errors }, reset, setError } = useForm<IServiceLogFormData>();
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
            if (data.date && data.expectedNextDate && data.date > data.expectedNextDate) {
                setError('expectedNextDate', { type: 'manual', message: t('AddMaintenanceLog.NextDateGTDate.Required')});
                hasError = true;
            }

            // Validate Odometer Readings
            if (data.odometerReading && data.expectedNextOdometerReading && data.odometerReading > data.expectedNextOdometerReading) {
                setError('expectedNextOdometerReading', { type: 'manual', message: t('AddMaintenanceLog.NextOdometerReadingGTOdometerReading.Required')});
                hasError = true;
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
                    data.type,
                    data.description,
                    data.date.toISOString(), 
                    data.expectedNextDate ? data.expectedNextDate.toISOString() : null, 
                    data.odometerReading,
                    data.expectedNextOdometerReading,
                    data.createdby,
                    data.phonenumber,
                    data.emailaddress,
                    file?.name || '',
                    null, 
                    file ? { data: file, fileName: file?.name || '' } : null
                );
                console.log(response);
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
    return <Drawer
        anchor="right"
        open={drawerOpen}
        onClose={toggleDrawer(false)}
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
                <IconButton onClick={toggleDrawer(false)}>
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
                {activeStep === 2 && <StepConfirmation control={control} />}
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
            case 3:
                return <PersonIcon color='primary' />;
            default:
                return <CheckIcon color='primary' />;
        }
    };

    return <div>{getIcon()}</div>;
};
