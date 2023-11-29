import React, { useEffect, useState } from "react";
import {
    Button,
    IconButton,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    TextField,
    useTheme,
    useMediaQuery,
    Select,
    InputAdornment,
    MenuItem,
    FormControl,
    InputLabel,
    CircularProgress
} from "@mui/material";
import { useTranslation } from "react-i18next";;
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import CloseIcon from '@mui/icons-material/Close';
import { Controller, useForm } from "react-hook-form";
import { UpdateVehicleServiceLogAsGarageCommand, GarageServiceType } from "../../../../app/web-api-client";
import useGarageServices from "../useGarageServicelogs";
import { getDefaultCreateGarageServices, getTitleForServiceType } from "../../defaultGarageService";

// own imports



interface IProps {
    mode: 'create' | 'edit';
    service?: UpdateVehicleServiceLogAsGarageCommand;
    dialogOpen: boolean;
    setDialogOpen: (dialogOpen: boolean) => void;
    createService: (data: any) => void;
    updateService: (data: any) => void;
    loading: boolean;
}

export default ({ dialogOpen, setDialogOpen, mode, service, createService, updateService, loading }: IProps) => {
    const { t } = useTranslation();
    const theme = useTheme();
    const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

    const [selectedService, setSelectedService] = useState<UpdateVehicleServiceLogAsGarageCommand | undefined>(service);
    const [dialogMode, setDialogMode] = useState<'create' | 'edit'>('create');
    const [timeUnit, setTimeUnit] = useState("minutes");


    const defaultAvailableServices = getDefaultCreateGarageServices(t);
    const { control, watch, setValue, handleSubmit, reset, formState: { errors }, setError } = useForm();

    useEffect(() => {
        if (mode === 'edit' && service) {
            setDialogMode('edit');

            setValue("serviceLogId", service.serviceLogId);
            setValue("vehicleLicensePlate", service.vehicleLicensePlate);
            setValue("type", service.type);
            setValue("description", service.description);
            setValue("date", service.date);
            setValue("expectedNextDate", service.expectedNextDate);
            setValue("odometerReading", service.odometerReading);
            setValue("expectedNextOdometerReading", service.expectedNextOdometerReading);
        }
        else {
            setDialogMode('create');
            reset();
        }
    }, [service, mode, setValue]);

    type ServiceLogProperty = 'vehicleLicensePlate' | 'type' | 'description' | 'date' | 'expectedNextDate' | 'odometerReading' | 'expectedNextOdometerReading';

    const handleTitleChange = (event: any) => {
        const service = defaultAvailableServices.find(item => item.type === event.target.value) as UpdateVehicleServiceLogAsGarageCommand;
        if (!service) return;

        const prevService = selectedService;
        setSelectedService(service);

        const item = watch();
        const propertiesToUpdate: ServiceLogProperty[] = ['vehicleLicensePlate', 'type', 'description', 'date', 'expectedNextDate', 'odometerReading', 'expectedNextOdometerReading'];
        propertiesToUpdate.forEach(property => {
            if (!item[property] || (prevService && item[property] == prevService[property])) {
                setValue(property, service[property]);
            }
        });
    };


    const onSubmit = (data: any) => {
        if (mode === 'edit') {
            updateService(data);
        } else {
            createService(data);
        }
    };

    return (
        <>
            <Dialog
                open={dialogOpen}
                onClose={() => setDialogOpen(false)}
                fullWidth
                maxWidth="sm"
                fullScreen={isMobile}
            >
                <DialogTitle>
                    {t(dialogMode === 'create' ? "service_add_title" : "service_edit_title")}
                    {isMobile && (
                        <IconButton onClick={() => setDialogOpen(false)} style={{ position: 'absolute', right: '8px', top: '8px' }}>
                            <CloseIcon />
                        </IconButton>
                    )}
                </DialogTitle>
                <form onSubmit={handleSubmit(onSubmit)}>
                    <DialogContent dividers>
                        <Controller
                            name="type"
                            control={control}
                            rules={{ required: t("What type of service do you need?") }}
                            defaultValue=""
                            render={({ field }) => (
                                <FormControl fullWidth variant="outlined" error={Boolean(errors.type)}>
                                    <InputLabel htmlFor="select-title">{t("Service type")}</InputLabel>
                                    <Select
                                        {...field}
                                        onChange={(event) => {
                                            field.onChange(event);
                                            handleTitleChange(event);
                                        }}
                                        label={t("Service type")}
                                    >
                                        {defaultAvailableServices.map(item => (
                                            <MenuItem key={item.type} value={item.type}>
                                                {getTitleForServiceType(t, item.type!)}
                                            </MenuItem>
                                        ))}
                                    </Select>
                                </FormControl>
                            )}
                        />
                        <Controller
                            name="description"
                            control={control}
                            rules={{ required: t("Description is required!") }}
                            defaultValue=""
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    label={t("Description")}
                                    fullWidth
                                    size="small"
                                    multiline
                                    rows={3}
                                    variant="outlined"
                                    error={Boolean(errors.description)}
                                    helperText={errors.description ? t(errors.description.message as string) : ' '}
                                    margin="normal"
                                />
                            )}
                        />
                        <Controller
                            name="durationInMinutes"
                            control={control}
                            rules={{ required: t("How much time does it take?") }}
                            defaultValue=""
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    label={t("Duration")}
                                    fullWidth
                                    size="small"
                                    type="number"
                                    inputProps={{ min: 0 }}
                                    variant="outlined"
                                    error={Boolean(errors.durationInMinutes)}
                                    helperText={errors.durationInMinutes ? t(errors.durationInMinutes.message as string) : ' '}
                                    margin="normal"
                                     
                                    InputProps={{
                                        startAdornment: (
                                            <InputAdornment position="start">
                                                <AccessTimeIcon />
                                            </InputAdornment>
                                        ),
                                        endAdornment: (
                                            <InputAdornment position="end">
                                                <Select
                                                    value={timeUnit}
                                                    onChange={(e) => setTimeUnit(e.target.value)}
                                                    sx={{
                                                        minWidth: "100%",
                                                        fontSize: '0.8rem',
                                                        border: 'none',
                                                        boxShadow: 'none',
                                                        '&:focus': {
                                                            border: 'none',
                                                            boxShadow: 'none',
                                                            outline: 'none',   // Remove the outline when focused
                                                        },
                                                        '& .MuiOutlinedInput-notchedOutline': {   // Remove the outline for the outlined variant
                                                            border: 'none',
                                                        },
                                                        '&:hover .MuiOutlinedInput-notchedOutline': {   // Remove the outline when hovered
                                                            border: 'none',
                                                        },
                                                    }}
                                                    size="small"
                                                >
                                                    <MenuItem value="minutes">{t("minutes")}</MenuItem>
                                                    <MenuItem value="hours">{t("hours")}</MenuItem>
                                                </Select>
                                            </InputAdornment>
                                        ),
                                        style: { paddingRight: '0' } // Reducing the padding to give more space
                                    }}
                                />
                            )}
                        />

                        <Controller
                            name="price"
                            control={control}
                            rules={{ required: t("How much is it?") }}
                            defaultValue=""
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    label={t("Price")}
                                    fullWidth
                                    size="small"
                                    type="number"
                                    inputProps={{ step: '0.01' }}
                                    variant="outlined"
                                    error={Boolean(errors.price)}
                                    helperText={errors.price ? t(errors.price.message as string) : ' '}
                                    margin="none"
                                    InputProps={{
                                        startAdornment: (
                                            <InputAdornment position="start">
                                                €
                                            </InputAdornment>
                                        ),
                                    }}
                                />
                            )}
                        />
                    </DialogContent>
                    <DialogActions>
                        <Button onClick={() => setDialogOpen(false)}>
                            {t("Cancel")}
                        </Button>
                        {loading ?
                            <Button variant="contained" disabled style={{ color: 'white' }}>
                                <CircularProgress size={24} color="inherit" />
                            </Button>
                            :
                            <Button type="submit" variant="contained" color="primary">
                                {t(dialogMode === 'create' ? "Create" : "edit")}
                            </Button>
                        }
                    </DialogActions>
                </form>
            </Dialog>

        </>
    );
}
